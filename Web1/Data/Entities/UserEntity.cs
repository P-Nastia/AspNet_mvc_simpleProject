using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web1.Data.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = String.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = String.Empty;

    [Required]
    [MaxLength(100)]
    public string Nickname { get; set; } = String.Empty;

    [Required, StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = String.Empty;

    [Required, StringLength(30)]
    public string Password { get; set; } = String.Empty;
    [Required, StringLength(13)]

    public string Phone { get; set; } = String.Empty;

    public string ImageUrl { get; set; } = String.Empty;
}
