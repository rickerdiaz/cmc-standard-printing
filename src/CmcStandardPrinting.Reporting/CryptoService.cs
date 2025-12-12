using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CmcStandardPrinting.Reporting
{
    /// <summary>
    /// Legacy-compatible crypto helper that mirrors the VB implementation used by DevExpress reports.
    /// Supports AES-128-CBC encryption/decryption with PKCS7 padding and the legacy 25-character hash format.
    /// </summary>
    public sealed class CryptoService
    {
        /// <summary>
        /// Optional environment secret used by the legacy <c>Set_Old</c> and <c>Get_Old</c> flows.
        /// </summary>
        public string? EnvironmentSecret { get; set; }

        public string? SetOld(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (string.IsNullOrEmpty(EnvironmentSecret))
            {
                throw new InvalidOperationException("EnvironmentSecret is not set.");
            }

            var key = Encoding.UTF8.GetBytes(EnvironmentSecret);
            var iv = Encoding.UTF8.GetBytes(EnvironmentSecret);
            ValidateKeyAndIv(key, iv);
            return EncryptAesCbcPkcs7(value!, key, iv);
        }

        public string? Set(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            const string k = "4a2b4c99c0bc1012";
            var key = Encoding.UTF8.GetBytes(k);
            var iv = Encoding.UTF8.GetBytes(k);
            ValidateKeyAndIv(key, iv);
            return EncryptAesCbcPkcs7(value!, key, iv);
        }

        public string? GetOld(string? encryptedBase64)
        {
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return encryptedBase64;
            }

            if (string.IsNullOrEmpty(EnvironmentSecret))
            {
                throw new InvalidOperationException("EnvironmentSecret is not set.");
            }

            var key = Encoding.UTF8.GetBytes(EnvironmentSecret);
            var iv = Encoding.UTF8.GetBytes(EnvironmentSecret);
            ValidateKeyAndIv(key, iv);
            return DecryptAesCbcPkcs7(encryptedBase64!, key, iv);
        }

        public string? Get(string? encryptedBase64)
        {
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return encryptedBase64;
            }

            const string k = "4a2b4c99c0bc1012";
            var key = Encoding.UTF8.GetBytes(k);
            var iv = Encoding.UTF8.GetBytes(k);
            ValidateKeyAndIv(key, iv);
            return DecryptAesCbcPkcs7(encryptedBase64!, key, iv);
        }

        /// <summary>
        /// Generates the legacy 25-character hash string using HMAC-SHA256, hex, BigInteger, and base-36 steps.
        /// </summary>
        public static string Get25CharString(string? code)
        {
            var hmacKey = Encoding.UTF8.GetBytes("6v3b6d88c0bc1045");
            var data = Encoding.UTF8.GetBytes(code ?? string.Empty);

            byte[] hashBytes;
            using (var hmac = new HMACSHA256(hmacKey))
            {
                hashBytes = hmac.ComputeHash(data);
            }

            var sb = new StringBuilder(hashBytes.Length * 2);
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            var hex = sb.ToString();
            var big = BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier);
            if (big.Sign < 0)
            {
                big = BigInteger.Negate(big);
            }

            var base36 = ToBase36(big);
            if (base36.Length == 0)
            {
                base36 = "0";
            }

            return base36.Length < 25
                ? base36.PadLeft(25, '0')
                : base36[^25..];
        }

        private static void ValidateKeyAndIv(byte[] key, byte[] iv)
        {
            if (key == null || key.Length != 16)
            {
                throw new ArgumentException("Key must be 16 bytes (AES-128).");
            }

            if (iv == null || iv.Length != 16)
            {
                throw new ArgumentException("IV must be 16 bytes (AES-128).");
            }
        }

        private static string EncryptAesCbcPkcs7(string plainText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var inputBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(inputBytes, 0, inputBytes.Length);
                cs.FlushFinalBlock();
            }

            var cipher = ms.ToArray();
            return Convert.ToBase64String(cipher);
        }

        private static string DecryptAesCbcPkcs7(string cipherBase64, byte[] key, byte[] iv)
        {
            var cipherBytes = Convert.FromBase64String(cipherBase64);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        private static string ToBase36(BigInteger value)
        {
            if (value.IsZero)
            {
                return "0";
            }

            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var sb = new StringBuilder();
            var v = BigInteger.Abs(value);

            while (v > BigInteger.Zero)
            {
                v = BigInteger.DivRem(v, 36, out var remainder);
                var idx = (int)remainder;
                sb.Insert(0, digits[idx]);
            }

            return sb.ToString();
        }
    }
}
