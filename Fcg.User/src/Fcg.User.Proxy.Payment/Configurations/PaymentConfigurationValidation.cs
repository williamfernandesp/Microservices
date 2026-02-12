using Microsoft.Extensions.Options;

namespace Fcg.User.Proxy.Payment.Configurations
{
    public class PaymentConfigurationValidation : IValidateOptions<PaymentConfiguration>
    {
        public ValidateOptionsResult Validate(string? name, PaymentConfiguration options)
        {
            if (string.IsNullOrWhiteSpace(options.Url))
            {
                return ValidateOptionsResult.Fail("Undefined 'Url' in configuration section 'PaymentConfiguration'");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
