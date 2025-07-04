using MediatR;
using Microsoft.AspNetCore.Authorization;
using PTM.Application.Commands;
using PTM.Application.DTOs.AuthDTOs;

namespace PTM.Server.Controllers
{
    [ApiController]
    [Route($"{ApiBase}\\[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost(AuthEndPoint.Login)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromForm] LoginRequestDto request, CancellationToken token)
        {
            return await sender.Send(new LoginUserCommand(request), token);
        }
        [HttpPost(AuthEndPoint.Register)]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromForm] RegisterRequestDto request, CancellationToken token)
        {
            return await sender.Send(new RegisterUserCommand(request), token);
        }

    }
}
