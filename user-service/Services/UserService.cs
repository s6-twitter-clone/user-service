﻿using user_service.Exceptions;
using user_service.Interfaces;
using user_service.Models;

namespace user_service.Services;

public class UserService
{
    private readonly IUnitOfWork unitOfWork;
    public UserService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
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
        if(Guid.TryParse(id, out _))
        {
            throw new BadRequestException($"Invalid id {id}");
        }

        if(unitOfWork.Users.GetById(id) is not null)
        {
            throw new BadRequestException("Account has already been initialized.");
        }

        if(string.IsNullOrWhiteSpace(displayName))
        {
            throw new BadHttpRequestException("Display name cannot by empty.");
        }

        var user = new User
        {
            Id = id,
            DisplayName = displayName,
            Bio = ""
        };

        unitOfWork.Users.Add(user);
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

        unitOfWork.Commit();

        return user;
    }
}
