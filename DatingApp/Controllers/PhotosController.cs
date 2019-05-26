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
    }
}
