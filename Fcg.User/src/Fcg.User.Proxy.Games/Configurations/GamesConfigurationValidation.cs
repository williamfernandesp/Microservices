using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Games.Configurations
{
    public class GamesConfigurationValidation : IValidateOptions<GamesConfiguration>
    {
        public ValidateOptionsResult Validate(string? name, GamesConfiguration options)
        {
            if (string.IsNullOrWhiteSpace(options.Url))
            {
                return ValidateOptionsResult.Fail("Undefined 'Url' in configuration section GamesConfiguration");
            }
            return ValidateOptionsResult.Success;
        }
    }
}
