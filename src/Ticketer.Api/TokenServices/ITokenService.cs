// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.Entities;


namespace Ticketer.Api.TokenServices
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, List<string> roles);
        Task<bool> IsCurrentActiveToken();
        System.Threading.Tasks.Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        System.Threading.Tasks.Task DeactivateAsync(string token);
        Task<bool> ConfirmDeactivatedTokenAsync();
    }
}
