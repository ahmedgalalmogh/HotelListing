using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(1,5)]

        public int Rating { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        public int countryId { get; set; }
    }
    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        public CountryDTO Country { get; set; }



    }
}
