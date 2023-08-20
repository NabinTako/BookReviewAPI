

using BookReview.Models;
using BookReview.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Services.BookServices {
	public class BookService : IBookService {

		private readonly DataContext _dbContext;
		public BookService(DataContext dbContext) {
			_dbContext = dbContext;

		}
		public async Task<ServerResponse<List<BookDto>>> GetAllBooks() {
			var response = new ServerResponse<List<BookDto>>();
			var bookList = await _dbContext.Books.Include(b => b.Tags).ToListAsync();
			response.Data = BookListToBookDtoListConverter(await _dbContext.Books.ToListAsync());
			return response;
		}
		public async Task<ServerResponse<BookDto>> GetBookById(int id) {
			var response = new ServerResponse<BookDto>();
			var book = await _dbContext.Books.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
			if (book == null) {
				response.Sucess = false;
				response.Message = $"Book with id '{id}' notfound";
				return response;
			}
			response.Data = new BookDto {
				Name = book.Name,
				Aurthor = book.Aurthor,
				Description = book.Description,
				Tags = BookTags(book)
			};
			return response;
		}
		public async Task<ServerResponse<List<BookDto>>> GetBookByTag(string tag) {
			List<BookDto> filteredBook = new List<BookDto>();
			var response = new ServerResponse<List<BookDto>>();
			var books = _dbContext.Books.Include(b => b.Tags).ToList();
			foreach (var book in books) {
				if (book.Tags.Count > 0) {
					foreach (var bookTag in book.Tags) {
						if (bookTag.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)) {
							filteredBook.Add(new BookDto {
								Name = book.Name,
								Aurthor = book.Aurthor,
								Description = book.Description,
								Tags = BookTags(book)
							});
						}
					}
				}
			}
			response.Data = filteredBook;

			if (response.Data.Count == 0) {
				response.Sucess = false;
				response.Message = $"Books with tag '{tag}' not found";
				return response;
			}
			return response;
		}
		public async Task<ServerResponse<List<BookDto>>> AddBook(BookDto newBook) {
			var response = new ServerResponse<List<BookDto>>();
			List<Tag> bookTags = new List<Tag>();
			foreach (var tag in newBook.Tags) {
				bookTags.Add(new Tag { Name = tag });
			}
			var bookToAdd = new Book {
				Name = newBook.Name,
				Aurthor = newBook.Aurthor,
				Description = newBook.Description,
				Tags = bookTags
			};
			_dbContext.Books.Add(bookToAdd);
			await _dbContext.SaveChangesAsync();
			response.Data = BookListToBookDtoListConverter(await _dbContext.Books.Include(b => b.Tags).ToListAsync());
			return response;
		}

		public async Task<ServerResponse<List<BookDto>>> DeleteBook(int id) {
			var response = new ServerResponse<List<BookDto>>();
			Book? bookToDelete = await _dbContext.Books.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
			if (bookToDelete == null) {
				response.Sucess = false;
				response.Message = $"Book with id '{id}' notfound";
				return response;
			}
			_dbContext.Remove(bookToDelete);
			await _dbContext.SaveChangesAsync();
			response.Data = BookListToBookDtoListConverter(await _dbContext.Books.Include(b => b.Tags).ToListAsync());
			return response;
		}

		//------------------------------------ Convetrter functions ------------------------------------
		//
		//function to store tags inside the new books 
		private List<Tag> BookTagToStore(List<string> tagNames) {
			var tags = new List<Tag>();
			foreach (string tag in tagNames) {
				tags.Add(new Tag { Name = tag });
			}
			return tags;
		}
		// List Tags to show __________________
		private List<string> BookTags(Book book) {
			var tags = new List<string>();
			if (book.Tags.Count > 0) {
				foreach (var item in book.Tags!) {
					tags.Add(item.Name);
				}
			}
			return tags;
		}
		// Book view model Lists ______________________
		private List<BookDto> BookListToBookDtoListConverter(List<Book> bookList) {
			List<BookDto> booksDto = new List<BookDto>();

			foreach (var book in bookList) {
				booksDto.Add(new BookDto {
					Name = book.Name,
					Aurthor = book.Aurthor,
					Description = book.Description,
					Tags = BookTags(book)
				});
			}
			return booksDto;
		}

	}
}
