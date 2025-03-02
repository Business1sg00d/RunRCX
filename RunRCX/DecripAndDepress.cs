using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;

namespace DecripAndDepress
{
    public class DecripAndDepress
    {
        public static byte[] DecompressGzip(byte[] compressedData)
        {
            using (var compressedStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                gzipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        public static byte[] Decrip(byte[] compressedData)
        {
            // Decrypt gzip data.
            Aes aes = Aes.Create();
            byte[] key = new byte[16] { 0x64, 0x26, 0x16, 0x83, 0xa9, 0x5d, 0xcf, 0xc6, 0xa8, 0x4c, 0x75, 0xf1, 0x42, 0x3a, 0x1c, 0x88 };
            byte[] iv = new byte[16] { 0x5b, 0xb8, 0x47, 0x7f, 0xc0, 0x83, 0x10, 0xc9, 0x2b, 0x83, 0xec, 0x3f, 0x16, 0x1b, 0x6f, 0x7f };
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            byte[] dGzip;
            using (var msDecrypt = new System.IO.MemoryStream(compressedData))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var msPlain = new System.IO.MemoryStream())
                    {
                        csDecrypt.CopyTo(msPlain);
                        dGzip = msPlain.ToArray();
                    }
                }
            }
            return dGzip;
        }
    }
}
