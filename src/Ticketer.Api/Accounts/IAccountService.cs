// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.Entities;
namespace Ticketer.Api.Accounts
{
    public interface IAccountService
    {

        /// <summary>
        /// Authenticates a user with the provided login credentials and returns an authentication response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task LogoutAsync(CancellationToken ct = default);

        /// <summary>
        /// Registers a new user with the specified details, optionally assigning roles and signing them in.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="roles"></param>
        /// <param name="signIn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> RegisterAsync(RegisterRequest request, IEnumerable<string>? roles = null, bool signIn = false, CancellationToken ct = default);

        /// <summary>
        /// Creates a new role with the specified name if it does not already exist. 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> CreateRoleAsync(string roleName, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<User>> GetUsersAsync(CancellationToken ct = default);

        /// <summary>
        /// Confirms a user's email address using the provided user ID and confirmation token. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default);

        /// <summary>
        /// Changes the password for the specified user, given their current password and a new password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken ct = default);

        /// <summary>
        /// Initiates the forgot password process for a user by generating a reset token and optionally creating a reset link using the provided link factory.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="linkFactory"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> ForgotPasswordAsync(string email, Func<string, string, string>? linkFactory = null, CancellationToken ct = default);

        /// <summary>
        /// Resets the password for a user using the provided reset token and new password.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

        /// <summary>
        /// Generates an email confirmation token for the specified user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default);
    }
}
