
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
            config.NewConfig<BookDto, Book>();
            config.NewConfig<Book, BookViewDto>().Map(dest => dest.isAvailable, src => src.CurrentQty>0 ? true : false);
            config.NewConfig<BorrowedBook, BorrowedBookViewDto>()
                        .Map(dest => dest.BorrowId, src => src.Id)
                        .Map(dest => dest.BookId, src => src.BookId)
                        .Map(dest => dest.BookName, src => src.Book.BookName)
                        .Map(dest => dest.Author, src => src.Book.Author)
                        .Map(dest => dest.Genre, src => src.Book.Genre);

        }
    }
}
