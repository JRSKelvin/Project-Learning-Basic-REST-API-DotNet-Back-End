using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Common;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            try
            {
                var result = await _authService.SignUpAsync(request);
                var response = new SuccessResponse<object>((int)HttpStatusCode.Created, "Sign Up Successfully", result);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse((int)HttpStatusCode.BadRequest, "Sign Up Failed", ex.Message);
                return StatusCode(error.StatusCode, error);
            }
            /*
            var response = await _authService.SignUpAsync(request);
            return Ok(response);
            */
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            try
            {
                var result = await _authService.SignInAsync(request);
                var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Sign In Successfully", result);
                return StatusCode(response.StatusCode, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var error = new ErrorResponse((int)HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
                return StatusCode(error.StatusCode, error);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse((int)HttpStatusCode.BadRequest, "Sign In Failed", ex.Message);
                return StatusCode(error.StatusCode, error);
            }
            /*
            var response = await _authService.SignInAsync(request);
            return Ok(response);
            */
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.RefreshToken);
                var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Token Refreshed Successfully", result);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse((int)HttpStatusCode.Unauthorized, "Invalid Refresh Token", ex.Message);
                return StatusCode(error.StatusCode, error);
            }
            /*
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
            */
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInformationFromToken()
        {
            try
            {
                var result = await _authService.GetCurrentUserMeAsync(User);
                var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "User Information Retrieved Successfully", result);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse((int)HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
                return StatusCode(error.StatusCode, error);
            }
            /*
            var response = await _authService.GetCurrentUserMeAsync(User);
            return Ok(response);
            */
        }
    }
}
