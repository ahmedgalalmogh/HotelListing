using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    public class LoginUserDTO
    {
  
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "passlimits error", MinimumLength = 10)]
        public string Password { get; set; }


    }
    public class UserDTO: LoginUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public ICollection<string> Roles { get; set; }



    }

}
