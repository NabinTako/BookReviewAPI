using BookReview.Models;
using BookReview.Services.BookServices;
using BookReview.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BookReview.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class BookController : ControllerBase {

		private readonly IBookService _bookService;

		public BookController(IBookService bookService) {
			_bookService = bookService;
		}

		[HttpGet("getall")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> GetAllBooks() {
			var results = await _bookService.GetAllBooks();
			return Ok(results);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<ServerResponse<BookDto>>> GetBookById(int id) {
			var results = await _bookService.GetBookById(id);
			if (results.Data == null) {
				return NotFound(results);
			}
			return results;
		}
		[HttpGet("tag/{tag}")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> GetBookByTag(string tag) {
			var results = await _bookService.GetBookByTag(tag); 
			if (results.Data == null) {
				return NotFound(results);
			}
			return Ok(results);
		}
		[HttpPost("add")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> AddBook(BookDto newBook) {
			var results = await _bookService.AddBook(newBook);
			if (results.Data == null) {
				return NotFound(results);
			}
			return Ok(results);
		}
		[HttpDelete("{id}")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> DeleteBook(int id) {
			var results = await _bookService.DeleteBook(id);
			if (results.Data == null) {
				return NotFound(results);
			}
			return results;
		}

	}
}
