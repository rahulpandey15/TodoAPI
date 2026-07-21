using System.Net.Http.Json;
using Todo.Application.Constants;
using Todo.Application.Contracts;

namespace Todo.Application.Implementation
{
    public class EmailService(
        IHttpClientFactory clientFactory) : IEmailService
    {
        public async Task<bool> SendMailAsync(Message message)
        {
            using var httpClient = 
                clientFactory.CreateClient(
                    ApplicationConstants.EmailServiceClient);
            
            var response = 
                await httpClient.PostAsJsonAsync(
                    "/email/send", message);
            
            response.EnsureSuccessStatusCode();
            
            return true;
        }
    }
}
