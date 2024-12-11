using System.Security.Cryptography;
using System.Text;

namespace BlazorSecurity.Encryption;

public class AsymmetricEncryptionHandler
{
    public string PublicKey { get; }
    
    private readonly string _privateKey;
    
    public AsymmetricEncryptionHandler(KeyManager keyManager)
    {
        using var rsa = new RSACryptoServiceProvider();

        (_privateKey, PublicKey) = keyManager.LoadKeys();
    }

    public string Encrypt(string plainText)
    {
        using var rsa = new RSACryptoServiceProvider();

        rsa.FromXmlString(PublicKey);
        return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), false));
    }

    public string Decrypt(string encryptedText)
    {
        using var rsa = new RSACryptoServiceProvider();

        rsa.FromXmlString(_privateKey);
        return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedText), false));
    }
}