using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class UserEntity : IdentityUser<Guid>
{
    public new string UserName
    {
        get => base.UserName ?? "";
        set => base.UserName = value;
    }
}