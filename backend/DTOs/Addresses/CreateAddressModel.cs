using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Addresses
{
    public class CreateAddressModel
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
