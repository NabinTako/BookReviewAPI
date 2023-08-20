namespace BookReview.ViewModels {
	public class BookDto {

		public string? Name { get; set; }
		public string? Aurthor { get; set; }
		public string? Description { get; set; }
		public List<string>? Tags { get; set; } = new List<string>();
	}
}
