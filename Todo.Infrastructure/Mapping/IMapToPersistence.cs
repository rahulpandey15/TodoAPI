using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Infrastructure.Mappers
{

    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
    }
}
