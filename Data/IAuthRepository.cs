namespace BookReview.Data {
	public interface IAuthRepository {

		public Task<ServerResponse<int>> Register(User user, string password);
		public Task<ServerResponse<string>> Login(string username, string password);
		public Task<bool> UserExists(string username);
	}
}
