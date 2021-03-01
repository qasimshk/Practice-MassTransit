using MassTransit;

namespace practice.one.api
{
    public static class CustomConfigurationExtensions
    {
        /// <summary>
        /// Should be used on every AddMassTransit configuration
        /// </summary>
        /// <param name="configurator"></param>
        /// 
        public static void ApplyCustomMassTransitConfiguration(this IBusRegistrationConfigurator configurator)
        {
            configurator.SetEndpointNameFormatter(new CustomEndpointNameFormatter());
        }
    }
}
