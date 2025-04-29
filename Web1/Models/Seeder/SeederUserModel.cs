using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Seeder;

public class SeederUserModel
{
    public string FirstName { get; set; } = String.Empty;

    public string LastName { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;

    public string PhoneNumber { get; set; } = String.Empty;

    public string Image { get; set; } = String.Empty;
}
