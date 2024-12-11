namespace BlazorSecurity.Encryption;

using System;
using System.Security.Cryptography;
using System.Text;

public class Hasher
{
    public static readonly byte[] Salt = "salt"u8.ToArray();

    // 1. SHA2
    public static T ComputeSHA2<T>(T input)
    {
        var inputBytes = ConvertToByteArray(input);
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(inputBytes);
        
        return ConvertFromByteArray<T>(hashBytes);
    }

    // 2. HMAC
    public static (T, byte[]?) ComputeHMAC<T>(T input, byte[]? key = null)
    {
        key ??= GenerateRandomSalt(input);
        
        var inputBytes = ConvertToByteArray(input);
        
        using var hmac = new HMACSHA256(key);
        var hashBytes = hmac.ComputeHash(inputBytes);
        
        return (ConvertFromByteArray<T>(hashBytes), key);
    }

    // 3. PBKDF2
    public static (T, byte[]) ComputePBKDF2<T>(T input, byte[]? salt = null, int iterations = 100, int hashLength = 32)
    {
        salt ??= GenerateRandomSalt(input);
        var inputBytes = ConvertToByteArray(input);
        
        using var rfc2898 = new Rfc2898DeriveBytes(inputBytes, salt, iterations, HashAlgorithmName.SHA256);
        
        return (ConvertFromByteArray<T>(rfc2898.GetBytes(hashLength)), salt);
    }
    
    // 4. BCRYPT
    public static dynamic ComputeBCrypt(dynamic input)
    {
        var inputString = ConvertToString(input);
        var hashString = BCrypt.Net.BCrypt.EnhancedHashPassword(inputString);
        
        return ConvertFromString(hashString, input.GetType());
    }

    public static bool VerifyBCrypt<T>(T input, string hash)
    {
        var inputString = ConvertToString(input);

        return BCrypt.Net.BCrypt.EnhancedVerify(inputString, hash);
    }

    // Utility Methods
    private static byte[] ConvertToByteArray<T>(T input)
    {
        return input switch
        {
            string strInput => Encoding.UTF8.GetBytes(strInput),
            byte[] byteInput => byteInput,
            _ => throw new ArgumentException("Input must be of type string or byte[]")
        };
    }

    private static string ConvertToString<T>(T input)
    {
        return input switch
        {
            string strInput => strInput,
            byte[] byteInput => Convert.ToBase64String(byteInput),
            _ => throw new ArgumentException("Input must be of type string or byte[]")
        };
    }

    private static T ConvertFromByteArray<T>(byte[] input)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)Convert.ToBase64String(input);
        }

        if (typeof(T) == typeof(byte[]))
        {
            return (T)(object)input;
        }

        throw new ArgumentException("Output type must be string or byte[]");
    }

    private static T ConvertFromString<T>(string input)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)input;
        }

        if (typeof(T) == typeof(byte[]))
        {
            return (T)(object)Convert.FromBase64String(input);
        }

        throw new ArgumentException("Output type must be string or byte[]");
    }
    
    private static dynamic ConvertFromString(string input, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return input;
        }

        if (targetType == typeof(byte[]))
        {
            return Convert.FromBase64String(input);
        }

        throw new ArgumentException("Output type must be string or byte[]");
    }
    
    private static byte[] GenerateRandomSalt<T>(T input)
    {
        var inputText = ConvertToString(input);
        
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[inputText.Length];
        rng.GetBytes(salt);
        return salt;
    }
}
