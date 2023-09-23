

using BookReview.Models;
using BookReview.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
			response.Data = BookListToBookDtoListConverter(bookList); //_dbContext.Books.ToListAsync()
			return response;
		}
		public async Task<ServerResponse<List<BookDto>>> StarredBooks(int userId) {
			var response = new ServerResponse<List<BookDto>>();
			try {
				var user = await _dbContext.Users.Include(u => u.StarredBooks!).ThenInclude(b => b.Tags).FirstOrDefaultAsync( u => u.Id == userId);
				var bookList = user.StarredBooks;
				//var bookList = await _dbContext.Books.Include(b => b.Tags).Where(b => b.StarredUsers!.Contains(user!)).ToListAsync();
				response.Data = BookListToBookDtoListConverter(bookList);
			}catch (Exception ex) {
				response.Sucess = false;
				response.Message = ex.Message;
			}
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
				Id = book.Id,
				Name = book.Name,
				Aurthor = book.Aurthor,
				Description = book.Description,
				Tags = BookTagsToStringList(book)
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
								Id = book.Id,
								Name = book.Name,
								Aurthor = book.Aurthor,
								Description = book.Description,
								Tags = BookTagsToStringList(book)
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
			//List<Tag> bookTags = new List<Tag>();
			//foreach (var tag in newBook.Tags) {
			//	bookTags.Add(new Tag { Name = tag });
			//}
			var bookToAdd = new Book {
				Name = newBook.Name,
				Aurthor = newBook.Aurthor,
				Description = newBook.Description,
				Tags = BookTagToStore(newBook.Tags)
			};
			 var addedBookRef = _dbContext.Books.Add(bookToAdd).Entity;
			addedBookRef.Details = new BookDetails { BookId = addedBookRef.Id };
			await _dbContext.SaveChangesAsync();
			response.Data = BookListToBookDtoListConverter(await _dbContext.Books.Include(b => b.Tags).ToListAsync());
			return response;
		}
		public async Task<ServerResponse<BookDto>> Starred(int bookId,int userId) {
			var response = new ServerResponse<BookDto>();
			var user = await _dbContext.Users.Include(u => u.StarredBooks).FirstOrDefaultAsync(u => u.Id == userId);
			var book = await _dbContext.Books.Include(b=> b.Tags).Include(b=> b.StarredUsers).FirstOrDefaultAsync(b => b.Id == bookId);
			if (user.StarredBooks.Contains(book)) {
				response.Sucess = false;
				response.Message = "Book already starred";
				return response;
			}
			book.StarredUsers.Add(user);
			user.StarredBooks.Add(book);
			await _dbContext.SaveChangesAsync();
			response.Data = new BookDto {
				Name = book.Name,
				Aurthor = book.Aurthor,
				Description = book.Description,
				Tags = BookTagsToStringList(book)
			};
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
			foreach (var tag in bookToDelete.Tags) {
				_dbContext.Tag.Remove(tag);
			}
			_dbContext.Books.Remove(bookToDelete);
			await _dbContext.SaveChangesAsync();
			response.Data = BookListToBookDtoListConverter(await _dbContext.Books.Include(b => b.Tags).ToListAsync());
			return response;
		}

		public async Task<ServerResponse<BookDetailsDto>> GetBookDetailsById(int id) {
			var response = new ServerResponse<BookDetailsDto>();
			var book = await _dbContext.Books.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
			if (book == null) {
				response.Sucess = false;
				response.Message = $"Book with id '{id}' notfound";
				return response;
			}
			var bookCommentsRef = await _dbContext.BookDetails.Include(b => b.Comments).FirstOrDefaultAsync(b => b.BookId == id);
			BookDetailsDto bookDetails = new BookDetailsDto() {
				Name = book.Name,
				Aurthor = book.Aurthor,
				Description = book.Description,
				Tags = BookTagsToStringList(book),
				Comments = BookCommentsToStringList(bookCommentsRef)
			}; 
			response.Data = bookDetails;
			return response;
		}
		public async Task<ServerResponse<AddCommentDto>>  AddComment(AddCommentDto comments) {
			var response = new ServerResponse<AddCommentDto>();
			var bookDetails = await _dbContext.BookDetails.FirstOrDefaultAsync(b => b.BookId == comments.BookId);
            if (bookDetails == null)
            {
                response.Sucess = false;
				response.Message = $"Book with id {comments.BookId} not found";
            }
            _dbContext.Comment.Add(new Comment {
				BookDetailsId = bookDetails.Id,
				comment = comments.Comment
			}) ;
			await _dbContext.SaveChangesAsync();
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
		private List<string> BookTagsToStringList(Book book) {
			var tags = new List<string>();
			if (book.Tags.Count > 0) {
				foreach (var item in book.Tags!) {
					tags.Add(item.Name);
				}
			}
			return tags;
		}
		// List of bookDetails comments
		private List<string> BookCommentsToStringList(BookDetails book) {
			var comments = new List<string>();
			if (book.Comments.Count > 0) {
				foreach (var item in book.Comments) {
					comments.Add(item.comment);
				}
			}
			return comments;
		}
		// Book view model Lists ______________________
		private List<BookDto> BookListToBookDtoListConverter(List<Book> bookList) {
			List<BookDto> booksDto = new List<BookDto>();

			foreach (var book in bookList) {
				booksDto.Add(new BookDto {
					Id = book.Id,
					Name = book.Name,
					Aurthor = book.Aurthor,
					Description = book.Description,
					Tags = BookTagsToStringList(book)
				});
			}
			return booksDto;
		}

	}
}
