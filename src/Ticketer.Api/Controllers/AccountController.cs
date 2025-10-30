// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketer.Api.Entities;
using Ticketer.Api.Accounts;

namespace Ticketer.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IAccountService accountService, [FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await accountService.LoginAsync(request, ct);
            if (!result.Succeeded) return Unauthorized(result.Errors);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromServices] IAccountService accountService, CancellationToken ct)
        {
            await accountService.LogoutAsync(ct);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromServices] IAccountService accountService, [FromBody] RegisterRequest request, [FromQuery] bool signIn = false, CancellationToken ct = default)
        {
            var result = await accountService.RegisterAsync(request, request.Roles, signIn, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRoles([FromServices] IAccountService accountService, [FromBody] List<string> roles, CancellationToken ct = default)
        {
            foreach (var item in roles)
            {
                var result = await accountService.CreateRoleAsync(item, ct);
                if (!result.Succeeded) return BadRequest(result.Errors);
           }

            return Ok();
        }

        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [Authorize(Roles = "User, Viewer, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromServices] IAccountService accountService, CancellationToken ct = default)
        {
            var users = new List<User>();
            var userList = await accountService.GetUsersAsync(ct);
            foreach (var item in userList)
            {
                users.Add(new User {
                    Id = item.Id,
                    UserName = item.UserName,
                    DisplayName = $"{item.FirstName} {item.LastName}"
                });
            }

            return Ok(users);
        }

        [Authorize(Roles = "User, Viewer, Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromServices] IAccountService accountService, [FromQuery] string userId, [FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var result = await accountService.ChangePasswordAsync(userId, request, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromServices] IAccountService accountService, [FromQuery] string email, CancellationToken ct)
        {
            var result = await accountService.ForgotPasswordAsync(email, null, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromServices] IAccountService accountService, [FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            var result = await accountService.ResetPasswordAsync(request, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromServices] IAccountService accountService, [FromQuery] string userId, [FromQuery] string token, CancellationToken ct)
        {
            var result = await accountService.ConfirmEmailAsync(userId, token, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromServices] IAccountService accountService, [FromQuery] string userId, CancellationToken ct)
        {
            var result = await accountService.GenerateEmailConfirmationTokenAsync(userId, ct);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Value);
        }
    }
}
