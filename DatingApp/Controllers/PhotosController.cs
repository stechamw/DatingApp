using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings>_cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper,
            IOptions<CloudinarySettings>cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;
            _mapper = mapper;
           
            // setup cloudinary account
           Account acc = new Account(
                _cloudinaryConfig.Value.CloudeName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            // create new instance of cloudinary and pass those account details
           _cloudinary = new Cloudinary(acc);
           
        }

        
        [HttpGet("{id}", Name = "GetPhoto")] // id of the photo
        public async Task<IActionResult>GetPhoto(int id)
        {

            var photoFromRepo = await _repo.GetPhoto(id);
            //we want photo to return
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);// because we don't want to return everything thats inside our PhotoFromRepo
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userid, 
            [FromForm/*where file is comming from*/]PhotoForCreationDto photoForCreationDto)
        {
            // UserId from Token matches userid from route
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized(); // if they don't match
            var userFromRepo = await _repo.GetUser(userid);
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();// this result comes from cloudinary dot net dot action
            // check and there is something inside the file
            if (file.Length > 0)
            {
                //read file into memory
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face") //detect face and crop
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreationDto);//photoForCreationDto is the map from
           

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            userFromRepo.Photos.Add(photo);
            // we can save it back to our repo
            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
            }
            return BadRequest("Could not add the photo");//if it doesn't work
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // first, we check to c the user is authorized

            // UserId from Token matches userid from route
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized(); // if they don't match
                                       // we want to check the user is updating one of there own photos
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
             //if the id we are passing dooes not match any of the photos in the collection
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();// return no content when saving was successful

            return BadRequest("Could not set photo to main");// else return bad request
        }

        [HttpDelete("{id}")] // id of the photo
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // we will not allow users to delete all photos, we will prompt them to upload replacement photo for Main
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized(); // if they don't match
            // we want to check the user is updating one of there own photos
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                //if the id we are passing dooes not match any of the photos in the collection
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");
            // we have our photo in cloudinary and we also have a reference record stored in our db
            // we will need to delete both of this
            // check if the photoFromRepo has a public id, if not then just delete from db and not from cloud
            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok") // check if deletion has suceeded at cloud 
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }
            
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}
