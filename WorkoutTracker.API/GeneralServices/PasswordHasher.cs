using Microsoft.AspNetCore.Identity;

namespace WorkoutTracker.API.GeneralServices
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<object> _hasher = new();

        public static string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public static bool Verify(string password, string hash)
        {
            return _hasher.VerifyHashedPassword(null!, hash, password)
                   == PasswordVerificationResult.Success;
        }
    }

}
