using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;

namespace AES_Encrypt_Byte_Buffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] baconFile;
            byte[] plaintextbuf;
            byte[] compressedBuff;
            byte[] encryptedData;
            string[] hexValues;

            // Option #1 is for a beacon. Option #2 is for a stager in byte[] format without brackets; I.e. "0x00,0x01,0x03...."
            if (args.Length < 1) {
                Console.WriteLine("Usage: .\\AES_Encrypt_Byte_Buffer.exe beaconBinary.exe 1");
                Console.WriteLine("Usage: .\\AES_Encrypt_Byte_Buffer.exe [file Conaining payload in byte[] format] 2");
                return;
            }

            if (args[1] == "1")
            {
                // Read in file as bytes:
                baconFile = System.IO.File.ReadAllBytes(args[0]);

                // GZIP data:
                compressedBuff = compressGzip(baconFile);

                // Encrypt gzip data:
                encryptedData = encFunc(compressedBuff);

                // Base64 Encode encryptedData, then write to a txt file:
                string b64 = Convert.ToBase64String(encryptedData);
                string currentDir = Directory.GetCurrentDirectory();
                string filePath = currentDir + "/" + "b64Output.txt";
                File.WriteAllText(filePath, b64);

            }
            else if (args[1] == "2")
            {
                // Read in file:
                hexValues = File.ReadAllText(args[0]).Split(',');
                
                // Allocate byte array and parse input file:
                plaintextbuf = parseCommas(hexValues);

                // GZIP data
                compressedBuff = compressGzip(plaintextbuf);

                // Encrypt gzip data:
                encryptedData = encFunc(compressedBuff);

                // Base64 Encode encryptedData, then write to a txt file:
                string b64 = Convert.ToBase64String(encryptedData);
                string currentDir = Directory.GetCurrentDirectory();
                string filePath = currentDir + "/" + "b64Output.txt";
                File.WriteAllText(filePath, b64);
            }
        }

        static byte[] compressGzip(byte[] Data)
        {
            using (MemoryStream freshStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(freshStream, CompressionMode.Compress))
                {
                    gzipStream.Write(Data, 0, Data.Length);
                }
                return freshStream.ToArray();
            }
        }

        static byte[] parseCommas(string[] hexValues)
        {
            byte[] plaintextbuf = new byte[hexValues.Length];

            // Parse each "0xNN" value
            for (int i = 0; i < hexValues.Length; i++)
            {
                string hex = hexValues[i].Trim(); // Remove any extra whitespace
                plaintextbuf[i] = Convert.ToByte(hex, 16); // Convert hex to byte
                
            }
            return plaintextbuf;
        }

        static byte[] encFunc(byte[] plaintextbuf) {

            // Encrypt shellcode.
            Aes aes = Aes.Create();
            byte[] key = new byte[16] { 0x64, 0x26, 0x16, 0x83, 0xa9, 0x5d, 0xcf, 0xc6, 0xa8, 0x4c, 0x75, 0xf1, 0x42, 0x3a, 0x1c, 0x88 };
            byte[] iv = new byte[16] { 0x5b, 0xb8, 0x47, 0x7f, 0xc0, 0x83, 0x10, 0xc9, 0x2b, 0x83, 0xec, 0x3f, 0x16, 0x1b, 0x6f, 0x7f };
            ICryptoTransform encrypter = aes.CreateEncryptor(key, iv);
            byte[] buf;
            using (var msEncrypt = new System.IO.MemoryStream(plaintextbuf))
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Read))
                {
                    using (var msPlain = new System.IO.MemoryStream())
                    {
                        csEncrypt.CopyTo(msPlain);
                        buf = msPlain.ToArray();
                    }
                }
            }
            return buf;
        }
    }
}
