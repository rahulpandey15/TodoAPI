namespace Todo.Application.Contracts;

public interface IPasswordHasher
{
    string Hash(string password);

    bool VerifyPassword(string password, string passwordHash);
}