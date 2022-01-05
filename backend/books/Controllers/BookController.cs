using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;

        public BookController(IBookService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync([FromForm] CreateBookDto createBookDto)
        {
            try
            {
                await _service.AddBookAsync(createBookDto);
                return Ok("Book created");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetBooksAsync()
        {
            var books = await _service.GetBooksAsync();

            return StatusCode(200, books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBookById(int id)
        {
            try
            {
                var book = await _service.GetBookByIdAsync(id);
                return StatusCode(200, book);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBookAsync(int id, [FromForm] UpdateBookDto updateBookDto)
        {
            try
            {
                await _service.UpdateBookAsync(id, updateBookDto);
                return Ok("Book Updated");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/name")]
        public async Task<ActionResult> FindAllBooksByNameAsync([FromQuery(Name = "name")]string name)
        {
            try
            {
                var books = await _service.FindAllBooksByNameAsync(name);
                return Ok(books);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBookAsync(int id)
        {
            await _service.DeleteBookAsync(id);

            return StatusCode(200, "Deleted");
        }
    }
}
