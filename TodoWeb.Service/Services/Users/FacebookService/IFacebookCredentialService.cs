using TodoWeb.Application.Dtos.UserModel.Contracts;

namespace TodoWeb.Service.Services.Users.FacebookService
{
    public interface IFacebookCredentialService
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);
        Task<FacebookUserInfoModel> GetUserInfoAsync(string accessToken);
    }
}
