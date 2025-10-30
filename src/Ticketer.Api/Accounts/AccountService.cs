// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ticketer.Api.Entities;
using Ticketer.Api.TokenServices;
using System.Net;

namespace Ticketer.Api.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signIn;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender? _email;

        public AccountService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signIn,
            ITokenService tokenService,
            IOptions<IdentityOptions> identityOptions,
            IEmailSender? email = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signIn = signIn;
            _tokenService = tokenService;
            _email = email;
        }

        /// <inheritdoc/>
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);
            if (user is null)
                return Result<AuthResponse>.Fail("Invalid credentials.");

            // Respect lockout policy and NotAllowed (e.g., email not confirmed)
            var check = await _signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (check.IsLockedOut) return Result<AuthResponse>.Fail("Account locked. Try again later.");
            if (check.IsNotAllowed) return Result<AuthResponse>.Fail("Sign-in not allowed (email not confirmed or policy blocked).");
            if (!check.Succeeded) return Result<AuthResponse>.Fail("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = TokenService.encodeJWTToken(this._tokenService.GenerateJwtToken(user, roles.ToList()), user.Email!, user.TwoFactorEnabled, 10);
            var resp = new AuthResponse(
                IsLoginSuccessful: true,
                AccessToken: token,
                Roles: roles,
                User: user
            );
            return Result<AuthResponse>.Ok(resp);
        }

        /// <inheritdoc/>
        public async Task LogoutAsync(CancellationToken ct = default)
        {
            // Signs out cookie-based sessions. For JWT, consider a jti denylist if you need server-side revocation.
            await _signIn.SignOutAsync();
            await _tokenService.DeactivateCurrentAsync();
        }

        /// <inheritdoc/>
        public async Task<Result> RegisterAsync(RegisterRequest request, IEnumerable<string>? roles = null, bool signIn = false, CancellationToken ct = default)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.EmailAddress,
                Email = request.EmailAddress,
                TwoFactorEnabled = false,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 3,
                DisplayName = $"{ request.FirstName} {request.LastName}",
                ClientName = "",
                BranchName = ""
            };

            if (roles is null || !roles.Any())
            {
                roles = new List<string> { "Viewer" };
            }

            var created = await _userManager.CreateAsync(user, request.Password);
            if (!created.Succeeded) return Result.FromIdentity(created);

            if (roles is not null && roles.Any())
            {
                var r = await _userManager.AddToRolesAsync(user, roles);
                if (!r.Succeeded) return Result.FromIdentity(r);
            }

            if (signIn)
            {
                // For cookie-based auth this would sign in. For pure JWT APIs, you typically just return a token via Login.
                await _signIn.SignInAsync(user, isPersistent: false);
            }
            return Result.Ok();
        }

          /// <inheritdoc/>
          public async Task<Result> CreateRoleAsync(string roleName, CancellationToken ct = default)
          {
               if (string.IsNullOrWhiteSpace(roleName))
                  return Result.Fail("Role name cannot be empty.");
               var exists = await _roleManager.RoleExistsAsync(roleName);
               if (exists)
                  return Result.Fail("Role already exists.");
               var result = await _roleManager.CreateAsync(new Role { Name = roleName, Enabled = true });
               return Result.FromIdentity(result);
          }

        /// <inheritdoc/>
        public async Task<List<User>> GetUsersAsync(CancellationToken ct = default)
        {
            var appUsers = _userManager.Users;
            if (appUsers == null)
                return new List<User>();

            await Task.CompletedTask;

            return appUsers.ToList();
        }

        /// <inheritdoc/>
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Fail("User not found.");

            var r = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result> ForgotPasswordAsync(string email, Func<string, string, string>? linkFactory = null, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal existence; return generic success
                return Result.Ok();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // If an email sender exists, send an email. The linkFactory builds a clickable URL.
            if (_email is not null && linkFactory is not null)
            {
                var urlSafeToken = WebUtility.UrlEncode(token);
                var link = linkFactory(email, urlSafeToken);
                await _email.SendEmailAsync(email, "Reset your password", $"Click the link to reset your password: {WebUtility.HtmlEncode(link)}");
            }

            // Optionally return the raw token in dev only; omitted here for safety.
            return Result.Ok();
        }

        /// <inheritdoc/>
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);
            if (user is null) return Result.Ok();  // generic success to avoid user enumeration

            var r = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Fail("User not found.");

            var r = await _userManager.ConfirmEmailAsync(user, token);
            return Result.FromIdentity(r);
        }

        /// <inheritdoc/>
        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result<string>.Fail("User not found.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result<string>.Ok(token);
        }
    }
}
