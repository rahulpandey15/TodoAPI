using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Application.Mappers;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
    public class UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService) : IUserService
    {

        public async Task<bool> CreateUserAsync(CreateUserDto userDto)
        {
            // dto into domain
            // user password into hashed password

            var userDomain = userDto.ToUserDomain();

            userDomain.PasswordHash = passwordHasher.Hash(userDto.Password);

            await userRepository.AddAsync(userDomain);

            var response = await userRepository.CommitAsync();

            if (response > 0)
                await emailService.SendMailAsync(
                    new Message(userDomain.Email,
                        EmailTemplate(userDomain.FullName)));

            return response > 0;
        }
        
        private string EmailTemplate(
            string userName)
        {
            return $"""
                    <html>
                    <head>

                    </head>
                    <body>
                        <div class="header">
                            <h1>Welcome to Our Todo App!</h1>
                        </div>
                        <div class="content">
                            <p>Hello {userName},</p>
                            <p>Thank you for joining our Todo application. We're excited to help you stay organized and productive.</p>
                            <p>Here's what you can do:</p>
                            <ul>
                                <li>Create and manage your daily tasks</li>
                                <li>Set priorities and deadlines</li>
                                <li>Track your progress</li>
                                <li>Collaborate with your team</li>
                            </ul>
                            <p>If you have any questions, feel free to reach out to our support team.</p>
                            <p>Best regards,<br>The Todo Team</p>
                        </div>
                        <div class="footer">
                            <p>&copy; 2026 Todo Application. All rights reserved.</p>
                        </div>
                    </body>
                    </html>
                    """;
        }
    }
}
