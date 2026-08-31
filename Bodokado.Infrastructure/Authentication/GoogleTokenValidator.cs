using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Bodokado.Application.Common.Auth;

namespace Bodokado.Infrastructure.Authentication;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthSettings _settings;

    public GoogleTokenValidator(Microsoft.Extensions.Options.IOptions<GoogleAuthSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<GoogleUserInfo?> ValidateAsync(string idToken)
    {
        try
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings { Audience = _settings.ValidClientIds };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
            return new GoogleUserInfo
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                PictureUrl = payload.Picture
            };
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
