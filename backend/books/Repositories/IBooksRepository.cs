using backend.Entities;

namespace backend.Repositories
{
    public interface IBooksRepository
    {
        Task AddBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> GetBookByNameAsync(string name);
        Task UpdateBookAsync(Book book);
        Task<IEnumerable<Book>> FindAllBooksByNameAsync(string name);
    }
}
