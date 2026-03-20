using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Addresses
{
    public class UpdateAddressModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string AddressLine { get; set; }

        public bool IsDefault { get; set; }
    }
}
