namespace BookReview.ViewModels {
	public class BookDetailsDto {
		public string? Name { get; set; }
		public string? Aurthor { get; set; }
		public string? Description { get; set; }
		public List<string>? Comments { get; set; } = new List<string>();
		public List<string>? Tags { get; set; } = new List<string>();
	}
}
