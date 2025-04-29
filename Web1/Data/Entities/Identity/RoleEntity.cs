using Microsoft.AspNetCore.Identity;

namespace Web1.Data.Entities.Identity;

public class RoleEntity : IdentityRole<int>
{
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
}
