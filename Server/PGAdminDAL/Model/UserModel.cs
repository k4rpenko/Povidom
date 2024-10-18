    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    namespace PGAdminDAL.Model
    {
        public class UserModel : IdentityUser
        {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }
<<<<<<< HEAD
        public List<string>? Subscribers { get; set; }
        public List<string>? Followers { get; set; }
=======
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }

    }
>>>>>>> 931bbafdd7a4930721c19131d50de09c66666db2
    }
