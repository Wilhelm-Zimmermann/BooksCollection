using backend.Database;
using backend.Dtos;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly DatabaseContext _context;
        public BooksRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Book book)
        {
            _context.Books.Attach(book);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> FindAllBooksByNameAsync(string name)
        {
            var books = await _context.Books.Where(x => x.Title.ToUpper()!.Contains(name.ToUpper())).ToListAsync();

            return books;
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Book> GetBookByNameAsync(string title)
        {
            return await _context.Books.Where(x => x.Title.ToUpper().Trim() == title.ToUpper().Trim()).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            var books = await _context.Books.OrderBy(x => x.Title).ToListAsync();

            return books;
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
