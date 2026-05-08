using System;
using System.Collections.Generic;
using System.Text;
using Todo.Application.DTO;

namespace Todo.Application.Interface
{
    public interface IUserService
    {
       Task<bool> AddUser(UserDto userDto);
    }
}
