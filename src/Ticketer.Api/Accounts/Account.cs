// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.AspNetCore.Identity;
using Ticketer.Api.Entities;


namespace Ticketer.Api.Accounts
{
    public record RegisterRequest(string UserName, string FirstName, string LastName, string Password, string? PhoneNumber, string EmailAddress, string Status, string? CapturedBy, DateTime? CapturedDate, List<string> Roles);
    public record LoginRequest(string UserName, string Password);
    public record ChangePasswordRequest(string UserName, string CurrentPassword, string NewPassword);
    public record ResetPasswordRequest(string UserName, string Token, string NewPassword);
    public record ConfirmEmailRequest(string UserName, string Token);
    public record AuthResponse(string AccessToken, bool IsLoginSuccessful, IEnumerable<string> Roles, User User);


    // Lightweight Result helpers
    public record Result(bool Succeeded, IEnumerable<string> Errors)
    {
        public static Result Ok() => new(true, Array.Empty<string>());
        public static Result Fail(params string[] errors) => new(false, errors);
        public static Result FromIdentity(IdentityResult id) => new(id.Succeeded, id.Errors.Select(e => e.Description));
    }
    public record Result<T>(bool Succeeded, T? Value, IEnumerable<string> Errors)
    {
        public static Result<T> Ok(T value) => new(true, value, Array.Empty<string>());
        public static Result<T> Fail(params string[] errors) => new(false, default, errors);
    }
}
