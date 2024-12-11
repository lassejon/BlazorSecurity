using System.Security.Cryptography;

namespace BlazorSecurity.Encryption;

public class KeyManager
{
    private static readonly string EncryptionKeysFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EncryptionKeys");
    private static readonly string PublicKeyPath = Path.Combine(EncryptionKeysFolder, "public.key");
    private static readonly string PrivateKeyPath = Path.Combine(EncryptionKeysFolder, "private.key");

    public static void InitializeKeys()
    {
        // Ensure the EncryptionKeys folder exists
        if (!Directory.Exists(EncryptionKeysFolder))
        {
            Directory.CreateDirectory(EncryptionKeysFolder);
        }

        // Check if the key files exist, and generate them if not
        if (!File.Exists(PublicKeyPath) || !File.Exists(PrivateKeyPath))
        {
            Console.WriteLine("Key files not found. Generating new keys...");
            GenerateAndSaveKeys();
        }
        else
        {
            Console.WriteLine("Key files found. Skipping key generation.");
        }
    }

    private static void GenerateAndSaveKeys()
    {
        using var rsa = new RSACryptoServiceProvider();
        
        var publicKey = rsa.ToXmlString(false);
        var privateKey = rsa.ToXmlString(true);
        
        File.WriteAllText(PublicKeyPath, publicKey);
        File.WriteAllText(PrivateKeyPath, privateKey);

        // Apply secure permissions
        ApplyFilePermissions(PublicKeyPath);
        ApplyFilePermissions(PrivateKeyPath);

        Console.WriteLine("Keys generated and saved.");
    }

    private static void ApplyFilePermissions(string filePath)
    {
        if (OperatingSystem.IsWindows())
        {
            // Set NTFS permissions (Windows)
            var fileInfo = new FileInfo(filePath);
            var security = fileInfo.GetAccessControl();
            security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
            var rule = new System.Security.AccessControl.FileSystemAccessRule(
                Environment.UserName,
                System.Security.AccessControl.FileSystemRights.FullControl,
                System.Security.AccessControl.AccessControlType.Allow
            );
            security.AddAccessRule(rule);
            fileInfo.SetAccessControl(security);
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            // Set POSIX permissions (Linux/macOS)
            System.Diagnostics.Process.Start("bash", $"-c \"chmod 600 {filePath}\"");
        }
    }

    public (string PrivateKey, string PublicKey) LoadKeys()
    {
        if (!File.Exists(PrivateKeyPath) || !File.Exists(PublicKeyPath))
        {
            throw new FileNotFoundException("Key files not found.");
        }

        string privateKey = File.ReadAllText(PrivateKeyPath);
        string publicKey = File.ReadAllText(PublicKeyPath);
        return (privateKey, publicKey);
    }
}