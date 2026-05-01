using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Infrastructure.Mappers
{
    public interface IMapToPersistence<TEntity>
    {
        TEntity ToEntity();
    }




    public static class MappingExtension
    {
        public static TEntity ToEntity<TEntity>(this  IMapToPersistence<TEntity> domain)
        {
            return domain.ToEntity();
        }

        public static  IEnumerable<TEntity> ToEntity<TEntity>(
            this IEnumerable<IMapToPersistence<TEntity>> domains)
        {
            return domains.Select(x => x.ToEntity());
        }
    }
}
