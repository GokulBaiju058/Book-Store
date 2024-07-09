using System.Security.Cryptography;

namespace BookStore_API.Business.Services.PasswordHasher
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public static string Hash(string password, int iterations, int hashSize)
        {
            byte[] salt = GenerateSalt(SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(hashSize);

            byte[] hashBytes = new byte[SaltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, hashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }

        public static string Hash(string password)
        {
            return Hash(password, 10000, HashSize);
        }

        public static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        public static bool Verify(string password, string hashedPassword)
        {
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hash type is not supported");
            }

            var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static byte[] GenerateSalt(int saltSize)
        {
            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
