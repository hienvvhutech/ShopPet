namespace Shopping_Pet.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string AddressLine { get; set; }

        public bool IsDefault { get; set; }
    }


}
