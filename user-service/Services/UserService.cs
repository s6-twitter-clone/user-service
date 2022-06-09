using user_service.Exceptions;
using user_service.Interfaces;
using user_service.Models;
using user_service.Models.Events;

namespace user_service.Services;

public class UserService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IEventService eventService;

    public UserService(IUnitOfWork unitOfWork, IEventService eventService)
    {
        this.unitOfWork = unitOfWork;
        this.eventService = eventService;
    }

    public User GetUserById(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) 
      {
            throw new BadRequestException("Id cannot be empty.");
      }

      var user = unitOfWork.Users.GetById(id); 

      if (user is null)
      {
            throw new NotFoundException($"User with id '{id}' doesn't exist.");
      }

      return user;
    }

    public User SetUpAccount(string id, string displayName)
    {
        if(string.IsNullOrWhiteSpace(id))
        {
            throw new BadRequestException($"Id cannot be empty.");
        }

        if(unitOfWork.Users.GetById(id) is not null)
        {
            throw new BadRequestException("Account has already been initialized.");
        }

        if(string.IsNullOrWhiteSpace(displayName))
        {
            throw new BadRequestException("Display name cannot be empty.");
        }

        var user = new User
        {
            Id = id,
            DisplayName = displayName,
            Bio = ""
        };

        unitOfWork.Users.Add(user);

        eventService.Publish(exchange: "user-exchange", topic: "user-added", new AddUserEvent { Id = id, DisplayName = displayName });

        unitOfWork.Commit();

        return user;
    }

    public User UpdateUser(string id, string displayName, string bio)
    {
        if(string.IsNullOrWhiteSpace(displayName))
        {
            throw new BadRequestException("Display name cannot be empty.");
        }
        
        
        var user = GetUserById(id);

        user.Bio = bio;
        user.DisplayName = displayName;

        eventService.Publish(exchange: "user-exchange", topic: "user-updated", new UpdateUserEvent
        {
            Id = id,
            DisplayName = displayName,
            Bio= bio
        });

        unitOfWork.Commit();

        return user;
    }
}
