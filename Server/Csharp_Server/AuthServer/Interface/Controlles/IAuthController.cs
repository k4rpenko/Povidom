using Microsoft.AspNetCore.Mvc;
using AuthServer.Models.Users;

namespace AuthServer.Interface.Controlles
{
    public interface IAuthController
    {
        Task<IActionResult> CreateUser(UserAuth _user);
        Task<IActionResult> LoginUser(UserAuth _user);
    }
}
