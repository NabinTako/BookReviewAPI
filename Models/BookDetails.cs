namespace BookReview.Models {
	public class BookDetails {
		public int Id { get; set; }
		public int BookId { get; set; }
		public Book Book { get; set; }
		public List<Comment>? Comments { get; set; }
	}
}
