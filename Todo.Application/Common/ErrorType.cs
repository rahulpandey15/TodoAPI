using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Application.Common
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Failure
    }
}
