using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Auth.Configurations
{
    public class AuthConfigurationValidation : IValidateOptions<AuthConfiguration>
    {
        public ValidateOptionsResult Validate(string? name, AuthConfiguration options)
        {
            if (string.IsNullOrWhiteSpace(options.Url))
            {
                return ValidateOptionsResult.Fail("Undefined 'Url' in configuration section AuthConfiguration");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
