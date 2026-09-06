using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Application.Contracts
{
    public interface ITokenRevocationService
    {
        Task<bool> IsSessionRevokedAsync(Guid sessionId);
        Task InvalidateSessionCacheAsync(Guid sessionId);
    }
}
