using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }//instead of date of birth
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }//when the profile was created
        public DateTime LastActive { get; set; }//to track when they were last active
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }//this will be used display main photo on our members profile
    }
}
