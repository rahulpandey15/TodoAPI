using System.Net.Http.Json;
using Todo.Application.Constants;
using Todo.Application.Contracts;

namespace Todo.Application.Implementation
{
    public class EmailService(
        IHttpClientFactory clientFactory) : IEmailService
    {
        public async Task<bool> SendMailAsync(string receipient)
        {
            using var httpClient = 
                clientFactory.CreateClient(
                    ApplicationConstants.EmailServiceClient);

            var response = 
                await httpClient.GetAsync(
                    $"/email/send?recipient={receipient}");
            
            response.EnsureSuccessStatusCode();
            
            return true;
        }
    }
}
