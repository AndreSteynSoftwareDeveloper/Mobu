using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Android.OS;
using Android.Util;

namespace MobuAndroid.Controls
{
    /// <summary>
    /// Utility used to encrypt and decrypt strings
    /// </summary>
    public static class EncryptionControl
    {
        /// <summary>
        /// Device serial number used as a unique hash.
        /// </summary>
        static readonly string Hash = Build.Serial;
        /// <summary>
        /// GUID used as encryption salt.
        /// </summary>
        static readonly string Salt = "DD2952B6-4DFE-4976-BF8D-85CD64DDC12B";
        /// <summary>
        /// Encryption key.
        /// </summary>
        static readonly string Key = "*4C6s8H4w9D2f8Z1";

        /// <summary>
        /// Encrypt strings using the unique device ID.
        /// </summary>
        /// <param name="stringToEncrypt">The string to be encrypted</param>
        public static string EncryptWithDeviceID(string stringToEncrypt)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(stringToEncrypt);

                byte[] keyBytes = new Rfc2898DeriveBytes(Hash, Encoding.ASCII.GetBytes(Salt)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
                var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(Key));

                byte[] cipherTextBytes;

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                        cryptoStream.Close();
                    }
                    memoryStream.Close();
                }
                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while encrypting string.", e);
                return String.Empty;
            }
        }

        /// <summary>
        /// Decrypt strings using  the unique device ID.
        /// </summary>
        /// <param name="stringToDecrypt">The string to be decrypted</param>
        public static string DecryptWithDeviceID(string stringToDecrypt)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(stringToDecrypt);
                byte[] keyBytes = new Rfc2898DeriveBytes(Hash, Encoding.ASCII.GetBytes(Salt)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(Key));
                var memoryStream = new MemoryStream(cipherTextBytes);
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception e)
            {
                Log.Error("Mobu", "Error while decrypting string.", e);
                return String.Empty;
            }
        }
    }
}
