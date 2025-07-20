namespace MyApi.Models 
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }  // Enum as string: Active, Inactive, Suspended
        // public string Role { get; set; }    // Enum as string: Customer, Manager, Admin
    }

    public class Name
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string Zipcode { get; set; }
        public Geolocation Geolocation { get; set; }
    }

    public class Geolocation
    {
        public string Lat { get; set; }
        public string Long { get; set; }
    }
}