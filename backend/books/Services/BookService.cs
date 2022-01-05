using backend.Dtos;
using backend.Entities;
using backend.Exceptions;
using backend.Repositories;
using backend.Utils;

namespace backend.Services
{
    public class BookService : IBookService
    {
        private readonly IBooksRepository _repository;
        private readonly IFileManager _fileManager;

        public BookService(IBooksRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        public async Task AddBookAsync(CreateBookDto createBookDto)
        {
            var formatedFileName = DateTime.Now.Ticks + createBookDto.Image.FileName;

            var bookTitleAlreadyExists = await _repository.GetBookByNameAsync(createBookDto.Title);

            if (bookTitleAlreadyExists != null)
            {
                throw new AppException("This title is already in use");
            }

            Book book = new()
            {
                Title = createBookDto.Title,
                Description = createBookDto.Description,
                Image = createBookDto.Image.FileName,
                Pages = createBookDto.Pages,
            };

            await _fileManager.SaveImage(createBookDto.Image, formatedFileName, "Books");

            await _repository.AddBookAsync(book);
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            var bookExists = await _repository.GetBookByIdAsync(id);

            if(bookExists is null)
            {
                throw new AppException("This book is invalid");
            }

            return bookExists;
        }

        public async Task<Book> GetBookByNameAsync(string name)
        {
            var book = await _repository.GetBookByNameAsync(name);

            if(book is null)
            {
                throw new AppException("Cannot find a book with this name");
            }

            return book;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _repository.GetBooksAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var bookExists = await _repository.GetBookByIdAsync(id);

            if (bookExists is null)
            {
                throw new AppException("This book is invalid");
            }

            await _repository.DeleteBookAsync(bookExists);
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            var formatedFileName = 
                  updateBookDto.Image != null 
                ? DateTime.Now.Ticks + updateBookDto.Image.FileName
                : "";

            var bookExists = await _repository.GetBookByIdAsync(id);

            if(bookExists is null)
            {
                throw new AppException("This book does not exists");
            }

            var bookTitleExists = await _repository.GetBookByNameAsync(updateBookDto.Title);

            if(bookTitleExists != null && bookTitleExists.Id != bookExists.Id)
            {
                throw new AppException("This title is already in use");
            }

            if(formatedFileName != "")
            {
                await _fileManager.RemoveImage(bookExists.Image, "Books");
            }

            bookExists.Title = updateBookDto.Title;
            bookExists.Description = updateBookDto.Description;
            bookExists.Pages = updateBookDto.Pages;
            bookExists.Image = formatedFileName == "" ? bookExists.Image : formatedFileName;

            await _repository.UpdateBookAsync(bookExists);
        }

        public async Task<IEnumerable<Book>> FindAllBooksByNameAsync(string name)
        {
            var books = await _repository.FindAllBooksByNameAsync(name);

            if(books is null)
            {
                throw new AppException("No books found with this name");
            }

            return books;
        }
    }
}
