namespace BookReview.Models {
	public class Comment {
		public int Id { get; set; }
		public string? comment { get; set; }
		public int BookDetailsId { get; set; }
		public BookDetails BookDetails { get; set; }
	}
}
