using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PGAdminDAL.Model;
using PGAdminDAL;
using Server.Controllers;
using Server.Hash;
using Server.Models.Users;
using Server.Sending;
using Server.Interface.Hash;
using Server.Interface.Sending;
using Microsoft.AspNetCore.Identity;
using Test.Additionally;
using Server.Interface.Controlles;

internal class AuthControllerTest
{
    private AuthController authController;
    private AppDbContext appDbContext;
    private Mock<IConfiguration> configuration;
    private Mock<IEmailSeding> emailSend;
    private Mock<IJwt> jwt;
    private Mock<IHASH> hash;
    private Mock<IRSAHash> rsa;
    private Email email = new Email();

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        configuration = new Mock<IConfiguration>();
        appDbContext = new AppDbContext(options, configuration.Object);
        emailSend = new Mock<IEmailSeding>();
        jwt = new Mock<IJwt>();
        hash = new Mock<IHASH>();
        rsa = new Mock<IRSAHash>();

        if (!appDbContext.Roles.Any(r => r.Name == "User"))
        {
            appDbContext.Roles.Add(new IdentityRole { Name = "User" });
            appDbContext.SaveChanges();
        }
        authController = new AuthController(appDbContext, emailSend.Object, jwt.Object, hash.Object, rsa.Object);
    }

    [Test]
    public async Task CreateUser_ReturnsBadRequest_WhenEmailOrPasswordIsEmpty()
    {
        var user = new UserAuth { Email = "", Password = "" };

        var result = await authController.CreateUser(user);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task CreateUser_ReturnsOk_WhenUserIsCreatedSuccessfully()
    {
        var result = await email.AddEmail(rsa, hash, jwt, emailSend, authController);

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task LoginUser_ReturnsBadRequest_WhenEmailOrPasswordIsEmpty()
    {
        var user = new UserAuth { Email = "", Password = "" };


        var result = await authController.LoginUser(user);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task LoginUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var user = new UserAuth { Email = "nonexistent@gmail.com", Password = "password" };

        var result = await authController.LoginUser(user);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }


    [Test]
    public async Task LoginUser_ReturnsOk_WhenCredentialsAreCorrect()
    {
        var user = new UserAuth { Email = "test@gmail.com", Password = "strinG123" };
        await email.AddEmail(rsa, hash, jwt, emailSend, authController);

        var result = await authController.LoginUser(user);

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [TearDown]
    public void TearDown()
    {
        authController.Dispose();
        appDbContext.Dispose();
    }
}
