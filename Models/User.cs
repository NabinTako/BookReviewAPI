namespace BookReview.Models {
	public class User {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public List<Book>? StarredBooks { get; set; }
    }
}
