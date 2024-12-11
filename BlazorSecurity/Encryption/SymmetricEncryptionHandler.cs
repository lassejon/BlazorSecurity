using Microsoft.AspNetCore.DataProtection;

namespace BlazorSecurity.Encryption;

public class SymmetricEncryptionHandler
{
    private const string SymmetricAlgorithmKey = "SymmetricAlgorithm";
    private readonly IDataProtector _dataProtector;

    public SymmetricEncryptionHandler(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtector = dataProtectionProvider.CreateProtector(SymmetricAlgorithmKey);
    }

    public string Encrypt(string plainText) => _dataProtector.Protect(plainText);
    
    public string Decrypt(string cipherText) => _dataProtector.Unprotect(cipherText);
}