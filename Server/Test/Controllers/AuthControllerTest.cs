using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PGAdminDAL;
using PGAdminDAL.Model;
using RedisDAL;
using Server.Controllers;
using Server.Hash;
using Server.Models.Users;
using Server.Sending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Test.Controllers
{
    internal class AuthControllerTest
    {
        private AuthController authController;
        private AppDbContext appDbContext;
        private Mock<IConfiguration> configuration;
        private Mock<EmailSeding> emailSend;
        private Mock<JWT> jwt;
        private Mock<HASH> hash;
        private Mock<RSAHash> rsa;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            configuration = new Mock<IConfiguration>();
            appDbContext = new AppDbContext(options, configuration.Object);
            emailSend = new Mock<EmailSeding>();
            jwt = new Mock<JWT>();
            hash = new Mock<HASH>();
            rsa = new Mock<RSAHash>();

            authController = new AuthController(appDbContext);
        }

        [Test]
        public async Task CreateUser_ReturnsBadRequest_WhenEmailOrPasswordIsEmpty()
        {
            var user = new UserAuth { Email = "", Password = "" };

            var result = await authController.CreateUser(user);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateUser_ReturnsOk()
        {
            var user = new UserAuth { Email = "test@gmail.com", Password = "strinG123" };

            var result = await authController.CreateUser(user);


            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [TearDown]
        public void TearDown()
        {
            appDbContext.Dispose();
            authController.Dispose();
        }
    }
}
