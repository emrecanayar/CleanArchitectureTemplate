using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers.Helpers
{
    public static class HashingHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string ToMD5(string str)
        {
            var encode = Encoding.UTF8;

            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(encode.GetBytes(str));
                var sb = new StringBuilder();
                foreach (var i in bytes)
                {
                    sb.Append(i.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string SHA1Encrypt(string content, Encoding encode)
        {
            try
            {
                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] bytes_in = encode.GetBytes(content);
                    byte[] bytes_out = sha1.ComputeHash(bytes_in);
                    string result = BitConverter.ToString(bytes_out);
                    result = result.Replace("-", string.Empty);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("SHA1：" + ex.Message);
            }
        }

        public static string AESEncrypt(string toEncrypt, string key)
        {
            if (string.IsNullOrWhiteSpace(toEncrypt))
            {
                return string.Empty;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = aes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        public static string AESDecrypt(string toDecrypt, string key)
        {
            if (string.IsNullOrWhiteSpace(toDecrypt))
            {
                return string.Empty;
            }

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = aes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
        }

        public static string GeyRandomAESKey()
        {
            StringBuilder str = new StringBuilder();
            Random rnd1 = new Random();
            int r = rnd1.Next(10, 100);
            long num2 = DateTime.Now.Ticks + r;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> r)));
            for (int i = 0; i < 16; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }

                str.Append(ch);
            }

            return str.ToString();
        }

        public static string GetMD5HashFromFile(Stream stream)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
