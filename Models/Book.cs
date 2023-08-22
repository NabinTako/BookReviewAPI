using Microsoft.EntityFrameworkCore;

namespace BookReview.Models {
	public class Book {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Aurthor { get; set; }
        public string? Description { get; set; }
        public BookDetails? Details { get; set; }
        public List<Tag>? Tags { get; set; }
    }
}
