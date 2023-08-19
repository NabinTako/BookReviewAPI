
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Services.BookServices {
	public interface IBookService {

		public Task<ServerResponse<List<Book>>> GetAllBooks();
		public Task<ServerResponse<List<Book>>> AddBook(Book newBook);
		public Task<ServerResponse<List<Book>>> DeleteBook(int id);
		public Task<ServerResponse<Book>> GetBookById(int id);
		public Task<ServerResponse<List<Book>>> GetBookByTag(Tag tag);
	}
}
