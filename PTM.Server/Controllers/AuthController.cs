using MediatR;
using Microsoft.AspNetCore.Authorization;
using PTM.Application.Commands;
using PTM.Application.DTOs.AuthDTOs;

namespace PTM.Server.Controllers
{
    [ApiController]
    [Route($"{ApiBase}/[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost(AuthEndPoints.Login)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromForm] LoginRequestDto request, CancellationToken token)
        {
            var result = await sender.Send(new LoginUserCommand(request), token);
            return new ObjectResult(result) { StatusCode = (int?)result.Code };
        }
        [HttpPost(AuthEndPoints.Register)]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromForm] RegisterRequestDto request, CancellationToken token)
        {
            var result = await sender.Send(new RegisterUserCommand(request), token);
            return new ObjectResult(result) { StatusCode = (int?)result.Code };
        }

    }
}
