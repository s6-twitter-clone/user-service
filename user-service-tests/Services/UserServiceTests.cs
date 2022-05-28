using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Exceptions;
using user_service.Interfaces;
using user_service.Models;
using user_service.Models.Events;
using user_service.Services;
using Xunit;

namespace user_service_tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> unitOfWork = new();
    private readonly Mock<IEventService> eventService = new();

    public UserServiceTests()
    {
        var user = new User();
        unitOfWork.Setup(x => x.Users.Add(user)).Returns(user);

        unitOfWork.Setup(x => x.Commit()).Returns(0);
    }

    [Fact]
    public void GetById_Success()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
            Bio = "bio"
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        var result = userService.GetUserById(user.Id);

        Assert.Equal(user, result);
    }

    [Fact]
    public void GetById_IdEmpty()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "",
            DisplayName = "name",
            Bio = "bio"
        };

        var result = Assert.Throws<BadRequestException>(() =>
            userService.GetUserById(user.Id)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Never);

        Assert.Equal("Id cannot be empty.", result.Message);
    }

    [Fact]
    public void GetById_UserNotFound()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
            Bio = "bio"
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(null as User);

        var result = Assert.Throws<NotFoundException>(() =>
            userService.GetUserById(user.Id)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);

        Assert.Equal($"User with id '{user.Id}' doesn't exist.", result.Message);
    }



    [Fact]
    public void SetUpAccount_Success()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
            Bio = ""
        };

        var result = userService.SetUpAccount(user.Id, user.DisplayName);

        unitOfWork.Verify(x => x.Commit(), Times.Once);



        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal(user.Bio, result.Bio);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.DisplayName, result.DisplayName);
    }

    [Fact]
    public void SetUpAccount_IdEmpty()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "",
            DisplayName = "name",
            Bio = ""
        };


        var result = Assert.Throws<BadRequestException>(() => userService.SetUpAccount(user.Id, user.DisplayName));

        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("Id cannot be empty.", result.Message);
    }

    [Fact]
    public void SetUpAccount_DisplayNameEmpty()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "",
            Bio = ""
        };


        var result = Assert.Throws<BadRequestException>(() => userService.SetUpAccount(user.Id, user.DisplayName));

        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("Display name cannot be empty.", result.Message);

    }

    [Fact]
    public void SetUpAccount_UserAlreadyExists()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "",
            Bio = ""
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        var result = Assert.Throws<BadRequestException>(() => userService.SetUpAccount(user.Id, user.DisplayName));

        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("Account has already been initialized.", result.Message);

    }

    [Fact]
    public void UpdateUser_Success()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
            Bio = ""
        };

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name",
            Bio = "bio"
        };


        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);


        var result = userService.UpdateUser(newUser.Id, newUser.DisplayName, newUser.Bio);

       
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal(newUser.Bio, result.Bio);
        Assert.Equal(newUser.Id, result.Id);
        Assert.Equal(newUser.DisplayName, result.DisplayName);
    }

    [Fact]
    public void UpdateUser_IdEmpty()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var newUser = new User
        {
            Id = "",
            DisplayName = "name",
            Bio = "bio"
        };

        var result = Assert.Throws<BadRequestException>(() =>
            userService.UpdateUser(newUser.Id, newUser.DisplayName, newUser.Bio)
        );

        unitOfWork.Verify(x => x.Users.GetById(newUser.Id), Times.Never);

        Assert.Equal("Id cannot be empty.", result.Message);
    }

    [Fact]
    public void UpdateUser_NotFound()
    {
        var userService = new UserService(unitOfWork.Object, eventService.Object);

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name",
            Bio = "bio"
        };

        var result = Assert.Throws<NotFoundException>(() => 
            userService.UpdateUser(newUser.Id, newUser.DisplayName, newUser.Bio)
        );


        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{newUser.Id}' doesn't exist.", result.Message);
    }
}
