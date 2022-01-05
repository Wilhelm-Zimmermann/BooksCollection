using backend.Dtos;
using backend.Entities;

namespace backend.Services
{
    public interface IBookService
    {
        Task AddBookAsync(CreateBookDto createBookDto);
        Task<IEnumerable<Book>> GetBooksAsync();
        Task DeleteBookAsync(int id);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> GetBookByNameAsync(string name);
        Task UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task<IEnumerable<Book>> FindAllBooksByNameAsync(string name);
    }
}
