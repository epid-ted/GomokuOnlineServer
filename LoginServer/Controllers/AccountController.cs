using LoginServer.Configuration;
using LoginServer.Data.DTOs.Client;
using LoginServer.Data.Models;
using LoginServer.Exceptions;
using LoginServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly SessionService sessionService;

        public AccountController(AccountService userService, SessionService sessionService)
        {
            this.accountService = userService;
            this.sessionService = sessionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            string username = loginRequestDto.Username;
            string password = loginRequestDto.Password;

            UserModel? userModel = await accountService.Authenticate(username, password);
            if (userModel == null)
            {
                return Unauthorized();
            }

            SessionModel sessionModel = await sessionService.EnterGame(userModel.UserId);

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                UserId = userModel.UserId,
                SessionId = sessionModel.SessionId,
                MatchServerAddress = ServerConfig.MatchServerPublicAddress,
                GameServerAddress = ServerConfig.GameServerPublicAddress
            };

            return Ok(loginResponseDto);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto logoutRequestDto)
        {
            int userId = logoutRequestDto.UserId;
            string sessionId = logoutRequestDto.SessionId;

            bool logoutSuccessed = await sessionService.LeaveGame(userId, sessionId);
            if (logoutSuccessed)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("register")]
        [TypeFilter(typeof(CustomExceptionFilter))]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            string username = registerRequestDto.Username;
            string password = registerRequestDto.Password;

            await accountService.Register(username, password);
            return Ok();
        }
    }
}
