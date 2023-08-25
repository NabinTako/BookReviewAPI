using Microsoft.EntityFrameworkCore;

namespace BookReview.Data {
	public class AuthRepository : IAuthRepository {

		private readonly DataContext _dataContext;

		public AuthRepository(DataContext dataContext)
        {
			_dataContext = dataContext;
		}
        public async Task<ServerResponse<string>> Login(string username, string password) {
			var response = new ServerResponse<string>();
			var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
			if(user == null) {

				response.Sucess = false;
				response.Message = "User not found";

			}else if(VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) == false) {

				response.Sucess = false;
				response.Message = "Wrong Password";

			} else {

				response.Data = user.Id.ToString();
			}
			return response;

		}

		public async Task<ServerResponse<int>> Register(User user, string password) {

			var response = new ServerResponse<int>();
			if (await UserExists(user.Username)) {
				response.Sucess = false;
				response.Message = "Users already exists";
				return response;

			}
			CreatePasswordHash(password, out byte[] PasswordHash, out byte[] PasswordSalt);

			user.PasswordSalt = PasswordSalt;
			user.PasswordHash = PasswordHash;

			_dataContext.Add(user);
			await _dataContext.SaveChangesAsync();
			response.Data = user.Id;
			return response;
		}

		public async Task<bool> UserExists(string username) {

			if(await _dataContext.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower()))) {
				return true;
			}
			return false;
		}

		private void CreatePasswordHash(string password, out byte[] PasswordHash, out byte[] PasswordSalt) {

			using (var hmac = new System.Security.Cryptography.HMACSHA512()) {

				PasswordSalt = hmac.Key;
				PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) { 
			
			using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {

				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

				return (computedHash.SequenceEqual(passwordHash));
			}
		
		}
	}
}
