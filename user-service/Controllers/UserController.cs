using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using user_service.Dtos;
using user_service.Services;

namespace user_service.Controllers;

[ApiController]
[Produces("application/json")]
[Route("")]
public class UserController : ControllerBase
{
    private readonly UserService userService;
    public UserController(UserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("{id}")]
    public UserDTO GetUser(string id)
    {
        var user = userService.GetUserById(id);

        return new UserDTO
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
        };
    }

    [HttpPut]
    [Authorize]
    public UserDTO updateUser(UpdateUserDTO user)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var updatedUser = userService.UpdateUser(userId, user.DisplayName, user.Bio);

        return new UserDTO
        {
            Id = updatedUser.Id,
            DisplayName = updatedUser.DisplayName,
            Bio = updatedUser.Bio,
        };
    }

    [HttpPost]
    [Authorize]
    public UserDTO setupAccount(CreateUserDTO user)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var newUser = userService.SetUpAccount(userId, user.DisplayName);

        return new UserDTO
        {
            Id = newUser.Id,
            DisplayName = newUser.DisplayName,
            Bio = newUser.Bio,
        };
    }
}
