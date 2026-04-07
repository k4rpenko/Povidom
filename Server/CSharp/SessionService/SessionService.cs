using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using PGAdminDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionService
{
    public class SessionServiceImpl : ISessionService
    {
        public readonly AppDbContext _context;
        public SessionServiceImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetUserIdAsync(HttpRequest req)
        {
            if (!req.Cookies.TryGetValue("_ASA", out string cookieValue)) return null;

            var session = await _context.Sessions.FirstOrDefaultAsync(u => u.KeyHash == cookieValue);

            if (session == null || session.LoginTime < DateTime.UtcNow)
            {
                if (session != null)
                {
                    _context.Sessions.Remove(session);
                    await _context.SaveChangesAsync();
                }
                return null;
            }

            return session.UserId;
        }

        public async Task<bool> IsSessionValidAsync(HttpRequest req)
        {
            var userId = await GetUserIdAsync(req);
            
            return userId != null;
        }

        public async Task<UserModel> GetUserDataAsync(HttpRequest req)
        {
            var userId = await GetUserIdAsync(req);

            if(userId == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        public async Task<UserModel> GetUserDataByIdAsync(string userId, HttpRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;
            
            return user;
        }
    }
}
