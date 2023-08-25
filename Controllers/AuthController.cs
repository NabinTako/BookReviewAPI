using BookReview.Data;
using BookReview.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase {

		private readonly IAuthRepository _authRepository;

		public AuthController(IAuthRepository authRepository)
        {
			_authRepository = authRepository;
		}

		[HttpPost("register")]
		public async Task<ActionResult<ServerResponse<int>>> Register(UserRegisterDto request) {
			var response = await _authRepository.Register(
				new User { Username = request.Username },request.Password);

			if(response.Sucess == false) {
				return BadRequest(response);
			}
			return Ok(response);
		}


		[HttpPost("login")]
		public async Task<ActionResult<ServerResponse<int>>> Login(UserLoginDto request) {
			var response = await _authRepository.Login(request.Username,request.Password);

			if (response.Sucess == false) {
				return BadRequest(response);
			}
			return Ok(response);
		}

	}
}
