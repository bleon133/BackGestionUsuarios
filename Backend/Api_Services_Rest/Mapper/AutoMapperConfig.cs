using AutoMapper;
using System;

namespace Api_Services_Rest.Mapper
{
    public static class AutoMapperConfig
    {
        public static IMapper Mapper { get; private set; }
        public static void Register()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = config.CreateMapper();
        }
    }
}