
using BookStore_API.Data.Entity;
using BookStore_API.Models;
using Mapster;
using System.Linq;

namespace BookStore_API.Business.Mapper
{
    public class GeneralMapper : IRegister
    {

        public void Register(TypeAdapterConfig config)
        {

            // This can be used for creating specific mapping configuration
            config.NewConfig<RegisterUserDto, User>();
            config.NewConfig<User, UserDto>();
        }
    }
}
