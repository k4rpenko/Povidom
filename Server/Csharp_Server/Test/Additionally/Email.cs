using Moq;
using Server.Controllers;
using Server.Interface.Hash;
using Server.Interface.Sending;
using Server.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test.Additionally
{
    internal class Email
    {
        public async Task<Microsoft.AspNetCore.Mvc.IActionResult> AddEmail(Mock<IRSAHash> rsa, Mock<IHASH> hash, Mock<IJwt> jwt, Mock<IEmailSeding> emailSend, AuthController authController)
        {
            var user = new UserAuth { Email = "test@gmail.com", Password = "strinG123" };
            hash.Setup(h => h.GenerateKey()).Returns(new byte[32]);
            hash.Setup(h => h.Encrypt(It.IsAny<string>(), It.IsAny<string>())).Returns("hashedPassword");
            rsa.Setup(r => r.GeneratePublicKeys()).Returns("publicKey");
            rsa.Setup(r => r.GeneratePrivateKeys()).Returns("privateKey");
            jwt.Setup(j => j.GenerateJwtToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns("token");
            emailSend.Setup(e => e.PasswordCheckEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            return await authController.CreateUser(user);
        }
    }
}
