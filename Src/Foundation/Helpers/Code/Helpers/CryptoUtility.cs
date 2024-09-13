using DEWAXP.Foundation.Logger;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DEWAXP.Foundation.Helpers
{
    public class CryptoUtility
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText))
                {
                    return null;
                }
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
                return string.Empty;
            }
        }

        /// <summary>
        /// The Base64Decode
        /// </summary>
        /// <param name="base64EncodedData">The base64EncodedData<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64EncodedData))
                {
                    return null;
                }

                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
                return string.Empty;
            }
        }

        /// <summary>
        /// will return hash of a password for storage in db
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Hashed string</returns>
        public static string EncryptPasswordForStorage(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// this will match the provided password with hashed one provided in storedPassword parameter
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedPassword"></param>
        /// <returns>True or False</returns>
        public static bool MatchWithEncryptedPassword(string password, string storedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(storedPassword);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, typeof(CryptoUtility));
                return false;
            }
            return true;
        }
    }
}