using Microsoft.AspNetCore.Mvc;
using Server.Models.Users;

namespace Server.Interface.Controlles
{
    public interface IAuthController
    {
        Task<IActionResult> CreateUser(UserAuth _user);
        Task<IActionResult> LoginUser(UserAuth _user);
    }
}
