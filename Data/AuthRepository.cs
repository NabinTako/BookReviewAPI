using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookReview.Data {
	public class AuthRepository : IAuthRepository {

		private readonly DataContext _dataContext;

		private readonly IConfiguration _configuration;

		public AuthRepository(DataContext dataContext, IConfiguration configuration)
        {
			_dataContext = dataContext;
			_configuration = configuration;
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

				response.Data = CreateToken(user);
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

		private string CreateToken(User user) {

			// user information to store in the token
			var claims = new List<Claim> {
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username)
			};
			var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
			if (appSettingsToken == null) {
				throw new Exception("AppSettings token is null");
			}

			SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
			SigningCredentials creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

			// object used to create token
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials = creds
			};

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			// making our token with the claims and objects
			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
