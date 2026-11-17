using System;
using System.Security.Cryptography;

namespace TraSuaMoc.Services
{
    public static class PasswordService
    {
        public static string Hash(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var key = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                return "100000." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(key.GetBytes(32));
        }
        public static bool Verify(string password, string stored)
        {
            try
            {
                var parts = stored.Split('.');
                var expected = Convert.FromBase64String(parts[2]);
                using (var key = new Rfc2898DeriveBytes(password, Convert.FromBase64String(parts[1]), int.Parse(parts[0]), HashAlgorithmName.SHA256))
                {
                    var actual = key.GetBytes(expected.Length);
                    int diff = 0;
                    for (int i = 0; i < actual.Length; i++) diff |= actual[i] ^ expected[i];
                    return diff == 0;
                }
            }
            catch (FormatException) { return false; }
        }
    }
}
