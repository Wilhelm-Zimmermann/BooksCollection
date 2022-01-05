using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using FluentAssertions;
using backend.Repositories;
using backend.Exceptions;
using backend.Entities;
using backend.Services;
using backend.Utils;
using backend.Dtos;

namespace backend.Books.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBooksRepository> _booksRepositoryMock = new();
        private readonly Mock<IFileManager> _fileManagerMock = new();
        private readonly Random _rand = new();

        [Fact]
        public async Task AddBookAsync_WithValidFields_ReturnsVoid()
        {
            var file = GenerateMockFile();

            var bookToCreate = new CreateBookDto
            {
                Title = "Alice in wonderland",
                Description = "Alice in a beautiful world",
                Pages = 200,
                Image = file
            };


            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            await service.AddBookAsync(bookToCreate);
        }

        [Fact]
        public async Task AddBookAsync_WithDuplicateTitle_ThrowsException()
        {
            var file = GenerateMockFile();
            var expectedBook = CreateBook();

            _booksRepositoryMock.Setup(repo => repo.GetBookByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedBook);

            var bookToCreate = new CreateBookDto
            {
                Title = "None of the above",
                Description = "None of the above",
                Pages = 200,
                Image = file
            };

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            
            var ex = await Assert.ThrowsAsync<AppException>(() => service.AddBookAsync(bookToCreate));

            Assert.Equal("This title is already in use",ex.Message);
        }

        [Fact]
        public async Task GetBookByIdAsync_WithExistingBook_ReturnsBook()
        {
            var expectedBook = CreateBook();
            _booksRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedBook);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            var result = await service.GetBookByIdAsync(_rand.Next(100));

            result.Should().BeEquivalentTo(
                    expectedBook,
                    options => options.ComparingByMembers<Book>()
            );
        }

        [Fact]
        public async Task GetBookByIdAsync_WithUnexistingBook_ThrowsException()
        {
            _booksRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Book)null);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            await Assert.ThrowsAsync<AppException>(async () => await service.GetBookByIdAsync(400));
        }

        [Fact]
        public async Task GetBooksAsync_WithExistingBooks_ReturnsBooks()
        {
            var books = new List<Book> { CreateBook(), CreateBook(), CreateBook()};
            _booksRepositoryMock.Setup(repo => repo.GetBooksAsync()).ReturnsAsync(books);
            
            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            var result = await service.GetBooksAsync();

            Assert.Equal(books, result);
        }

        [Fact]
        public async Task DeleteBookAsync_WithExistingBook_ReturnsVoid()
        {
            var expectedBook = CreateBook();
            _booksRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedBook);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            await service.DeleteBookAsync(1);

        }

        [Fact]
        public async Task DeleteBookAsync_WithUnexistingBook_ThrowsException()
        {

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            var ex = await Assert.ThrowsAsync<AppException>(() => service.DeleteBookAsync(1));

            Assert.Equal("This book is invalid", ex.Message);
        }

        [Fact]
        public async Task UpdateBookAsync_WithExistingBook_ReturnsVoid()
        {
            var book = CreateBook();
            _booksRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(book);
            _booksRepositoryMock.Setup(repo => repo.GetBookByNameAsync(It.IsAny<string>())).ReturnsAsync(book);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            var updateBook = new UpdateBookDto
            { 
                Title = "Hello",
                Description = "This is an update",
                Image = GenerateMockFile(),
                Pages = 123,
            };

            await service.UpdateBookAsync(1, updateBook);
        }

        [Fact]
        public async Task UpdateBookAsync_WithUnexistingBook_ThrowsException()
        {
            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            var updateBook = new UpdateBookDto
            {
                Title = "Hello",
                Description = "This is an update",
                Image = GenerateMockFile(),
                Pages = 123,
            };

            var ex = await Assert.ThrowsAsync<AppException>(() =>service.UpdateBookAsync(1, updateBook));

            Assert.Equal("This book does not exists", ex.Message);
        }

        [Fact]
        public async Task UpdateBookAsync_WithDuplicateTitle_ThrowsException()
        {
            var book = CreateBook();
            var bookTitle = new Book
            {
                Id = 6,
                Title = "Alice World",
                Description = "I don't know",
                Pages = 150,
                Image = "Enchanted.png",
                CreateDate = DateTime.UtcNow,
            };

            _booksRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(book);
            _booksRepositoryMock.Setup(repo => repo.GetBookByNameAsync(It.IsAny<string>())).ReturnsAsync(bookTitle);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);

            var updateBook = new UpdateBookDto
            {
                Title = "Alice World",
                Description = "This is an update",
                Image = GenerateMockFile(),
                Pages = 123,
            };

            var ex = await Assert.ThrowsAsync<AppException>(() => service.UpdateBookAsync(1, updateBook));

            Assert.Equal("This title is already in use", ex.Message);
        }

        [Fact]
        public async Task FindAllBooksByNameAsync_WithMatchingQuery_ReturnBooks()
        {
            var expectedBooks = new List<Book> { CreateBook(), CreateBook() };
            _booksRepositoryMock.Setup(repo => repo.FindAllBooksByNameAsync(It.IsAny<string>())).ReturnsAsync(expectedBooks);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            var books = await service.FindAllBooksByNameAsync("Qualquer Nome");
            Assert.Equal(expectedBooks, books);
        }

        [Fact]
        public async Task FindAllBooksByNameAsync_WithNonMatchingQuery_ThrowsException()
        {
            _booksRepositoryMock.Setup(repo => repo.FindAllBooksByNameAsync(It.IsAny<string>())).ReturnsAsync((List<Book>) null);

            var service = new BookService(_booksRepositoryMock.Object, _fileManagerMock.Object);
            var ex = await Assert.ThrowsAsync<AppException>(() => service.FindAllBooksByNameAsync("Qualquer Nome"));

            Assert.Equal("No books found with this name", ex.Message);
        }

        private Book CreateBook()
        {
            return new()
            {
                Id = _rand.Next(1000),
                Image = "String.jpeg",
                CreateDate = DateTime.Now,
                Description = "A beaultiful book",
                Pages = 123,
                Title = "None of the above"
            };

        }

        private IFormFile GenerateMockFile()
        {
            var fileMock = new Mock<IFormFile>();
            
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            return fileMock.Object;
        }
    }
}
