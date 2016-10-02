using CryptSharp;

namespace enBask.Core.Website.Authentication
{
    public static class PasswordHelper
    {
        public static string GenerateSalt(int size)
        {
            var salt = Crypter.Blowfish.GenerateSalt();
            return salt;
        }

        public static string HashPassword(string password, string salt)
        {
            var hashed = Crypter.Blowfish.Crypt(password, salt);
            return hashed;
        }

        public static bool doesPasswordMatch(string password, string salt, string hash)
        {
            var compare = Crypter.Blowfish.Crypt(password, salt);
            return (compare == hash);
        }
    }
}
