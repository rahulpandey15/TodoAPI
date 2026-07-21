namespace Todo.Application.Contracts
{
    public record Message(string recipient, string messageBody);


    public interface IEmailService
    {
        Task<bool> SendMailAsync(Message message);
    }
}
