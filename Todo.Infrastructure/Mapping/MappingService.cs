using Microsoft.Extensions.DependencyInjection;
using Todo.Infrastructure.Mappers;

namespace Todo.Infrastructure.Mapping
{

    public interface IMappingService
    {
        TDest Map<TSrc, TDest>(TSrc source);
    }

    public class MappingService : IMappingService
    {
        private readonly IServiceProvider _serviceProvider;

        public MappingService(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public TDest Map<TSrc, TDest>(TSrc source)
        {
            var mapper = _serviceProvider.GetService<IMapper<TSrc, TDest>>();

            if (mapper == null)
                throw new InvalidOperationException($"No mapper found for {typeof(TSrc).Name} to {typeof(TDest).Name}");

            return mapper.Map(source);
        }
    }
}
