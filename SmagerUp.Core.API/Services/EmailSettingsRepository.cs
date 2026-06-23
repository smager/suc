using SmagerUp.Core.API.Models;
namespace SmagerUp.Core.API.Services
{
    public class EmailSettingsRepository
    {
        private readonly SettingsRepository _settings;

        public EmailSettingsRepository(SettingsRepository settings)
        {
            _settings = settings;
        }

        public async Task<EmailSettings> GetInfoAsync()
        {
            var dict = await _settings.GetCategoryAsync("email");

            return new EmailSettings
            {
                Host = dict.GetValueOrDefault("Host", ""),
                Port = int.TryParse(dict.GetValueOrDefault("Port", "25"),out var port) ? port : 25,
                Address = dict.GetValueOrDefault("Address", ""),
                Password = dict.GetValueOrDefault("Password", ""),
                DisplayName = dict.GetValueOrDefault("DisplayName", ""),
                IsSSL = dict.GetValueOrDefault("IsSSL", "N").Equals("Y",StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
