
namespace BookReview.Services.BookServices {
	public class BookService : IBookService {

		private static List<Book> books = new List<Book>() {
			new Book(){Id=1,Name="h",Aurthor="test",Description="shdakj",Tags = new List<Tag>{ Tag.Romance } },
			new Book(){Id=2,Name="h2",Aurthor="test2",Description="shdakj2",Tags = new List<Tag>{ Tag.Sci_Fi, Tag.Romance }},
			new Book(){Id=3,Name="h3",Aurthor="test3",Description="shdakj3",Tags = new List<Tag>{ Tag.Horror } },
		};
		public async Task<ServerResponse<List<Book>>> AddBook(Book newBook) {
			var response = new ServerResponse<List<Book>>();
			books.Add(newBook);
			response.Data = books;
			return response;
		}

		public async Task<ServerResponse<List<Book>>> DeleteBook(int id) {
			var response = new ServerResponse<List<Book>>();
			Book bookToDelete = books.Find(b => b.Id == id)!;
			if (bookToDelete == null) {
				response.Sucess = false;
				response.Message = $"Book with id '{id}' notfound";
				return response;
			}
			books.Remove(bookToDelete);
			response.Data = books;
			return response;
		}

		public async Task<ServerResponse<List<Book>>> GetAllBooks() {
			var response = new ServerResponse<List<Book>>();
			response.Data = books;
			return response;
		}

		public async Task<ServerResponse<Book>> GetBookById(int id) {
			var response = new ServerResponse<Book>();
			response.Data = books.Find(b => b.Id == id);
			if (response.Data == null) {
				response.Sucess = false;
				response.Message = $"Book with id '{id}' notfound";
				return response;
			}
			return response;
		}

		public async Task<ServerResponse<List<Book>>> GetBookByTag(Tag tag) {
			var response = new ServerResponse<List<Book>>();
			response.Data = books.FindAll(b => b.Tags!.Contains(tag));
			if (response.Data.Count == 0) {
				response.Sucess = false;
				response.Message = $"Books with tag '{tag}' not found";
				return response;
			}
			return response;
		}
	}
}
