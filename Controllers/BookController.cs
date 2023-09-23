using BookReview.Models;
using BookReview.Services.BookServices;
using BookReview.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReview.Controllers {

	[Authorize]
	[ApiController]
	//[AllowAnonymous]
	[Route("[controller]")]
	public class BookController : ControllerBase {

		private readonly IBookService _bookService;

		public BookController(IBookService bookService) {
			_bookService = bookService;
		}

		[AllowAnonymous]
		[HttpGet("getall")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> GetAllBooks() {
			var results = await _bookService.GetAllBooks();
			return Ok(results);
		}

		[HttpGet("starred")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> StarredBooks() {
			int id = int.Parse( User.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier)!.Value );
			var results = await _bookService.StarredBooks(id);
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

		[HttpGet("details/{id}")]
		public async Task<ActionResult<ServerResponse<BookDetailsDto>>> GetBookDetailsById(int id) {
			var results = await _bookService.GetBookDetailsById(id);
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
		[HttpPost("addbook")]
		public async Task<ActionResult<ServerResponse<List<BookDto>>>> AddBook(BookDto newBook) {
			var results = await _bookService.AddBook(newBook);
			if (results.Data == null) {
				return NotFound(results);
			}
			return Ok(results);
		}

		[HttpPost("starbook")]
		public async Task<ActionResult<ServerResponse<BookDto>>> Starred(int bookId) {
			int userId = int.Parse( User.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier)!.Value );
			var results = await _bookService.Starred(bookId, userId);
			if (results.Data == null) {
				return NotFound(results);
			}
			return Ok(results);
		}
		[HttpPost("addcomment")]
		public async Task<ActionResult<ServerResponse<AddCommentDto>>> AddComment(AddCommentDto comment){
			var results = await _bookService.AddComment(comment);
			if(results.Sucess == false) {
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
