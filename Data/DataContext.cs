using Microsoft.EntityFrameworkCore;

namespace BookReview.Data {
	public class DataContext : DbContext {
		public DataContext(DbContextOptions<DataContext> dbContext) : base(dbContext) {

		}
		public DbSet<Book> Books => Set<Book>();
		public DbSet<Tag> Tag => Set<Tag>();
		public DbSet<BookDetails> BookDetails => Set<BookDetails>();
		public DbSet<Comment> Comment => Set<Comment>();
		public DbSet<User> Users=> Set<User>();
	}
}
