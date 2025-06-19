using Google.Apis.Auth;

namespace TodoWeb.Service.Services.Users.GoogleService
{
    public interface IGoogleCredentialService
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyCredential(string clientId, string credential);
        public Task<GoogleJsonWebSignature.Payload> VerifyCredential(string credential);
    }
}
