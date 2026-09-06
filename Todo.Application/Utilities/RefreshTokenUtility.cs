using System.Security.Cryptography;

namespace Todo.Application.Utilities
{
   
    public static class RefreshTokenUtility
    {
        public static string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenBytes = new byte[32]; // 256 bits
                rng.GetBytes(tokenBytes);

                // Use URL-safe base64 encoding
                return Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
        }
    }
}
