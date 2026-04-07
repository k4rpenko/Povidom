using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using PGAdminDAL;
using PGAdminDAL.Model;

namespace SessionService
{
    public interface ISessionService
    {
        Task<bool> IsSessionValidAsync(HttpRequest req);
        Task<string> GetUserIdAsync(HttpRequest req);
        Task<UserModel> GetUserDataAsync(HttpRequest req);
        Task<UserModel> GetUserDataByIdAsync(string userId, HttpRequest req);
    }
}
