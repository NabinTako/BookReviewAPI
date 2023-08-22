namespace BookReview.ViewModels {
	public class AddCommentDto {
		public int BookId { get; set; }
		public string Comment { get; set; } = "";
	}
}
