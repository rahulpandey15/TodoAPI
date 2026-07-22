using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Application.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(string recipient);
    }
}
