using System.ComponentModel.DataAnnotations;

namespace Web1.Models.Seeder;

public class SeederUserModel
{
    public string Name { get; set; } = String.Empty;

    public string Surname { get; set; } = String.Empty;
    public string Nickname { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;

    public string Phone { get; set; } = String.Empty;

    public string Image { get; set; } = String.Empty;
    public string Role { get; set; } = String.Empty;
}
