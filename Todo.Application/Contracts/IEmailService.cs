namespace Todo.Application.Contracts
{
    
    public interface IEmailService
    {
        Task<bool> SendMailAsync(string recipient);
    }
}
