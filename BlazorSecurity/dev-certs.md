# Trusting a Development Certificate for ASP.NET Core

This guide describes the process of setting up and trusting a development certificate for ASP.NET Core development.

---

## Steps to Set Up and Trust the Certificate

### 1. Clean Existing Certificates
Before creating a new certificate, clean up any existing development certificates:
```bash
dotnet dev-certs https --clean
````


### 2. Create and Export a New Certificate
Generate a new self-signed development certificate and export it with a specified password:
```bash
dotnet dev-certs https --export-path ~/.aspnet/dev-certs/https/BlazorSecurity.pfx --password Passw0rd!
```

### 3. Trust the Certificate
Trust the generated certificate to allow HTTPS connections during development:
```bash
dotnet dev-certs https --trust
```

---

## Additional Steps for macOS

On macOS, if you want to store the certificate password securely using ASP.NET Core’s **User Secrets**, you need to create a secrets file.

### 1. Create the User Secrets Directory
Create the `secrets.json` file in the following path:
```bash
~/.microsoft/usersecrets/{UserSecretsId}/secrets.json
```

Here:
- `{UserSecretsId}` is the `UserSecretsId` defined in your `.csproj` file. For example:
  ```xml
  <UserSecretsId>aspnet-BlazorSecurity-94687839-9975-4e81-8219-4750602385af</UserSecretsId>
  ```

### 2. Create and Edit `secrets.json`
Inside the folder, create a `secrets.json` file with the following content:
```json
{
    "KestrelCertificatePassword": "Passw0rd!"
}
```

If the `~/.microsoft/usersecrets` folder does not exist, create it manually:
```bash
mkdir -p ~/.microsoft/usersecrets/{UserSecretsId}
touch ~/.microsoft/usersecrets/{UserSecretsId}/secrets.json
```

---

## Additional Steps for Other Platforms

For other platforms (e.g., Windows or Linux), the `secrets.json` file can also be used. Follow the same structure:
1. Locate the secrets folder (typically `~/.microsoft/usersecrets` on Linux or `%APPDATA%\Microsoft\UserSecrets` on Windows).
2. Create or edit the `secrets.json` file with the same content:
   ```json
   {
       "KestrelCertificatePassword": "Passw0rd!"
   }
   ```

---

## Using the Password in ASP.NET Core

To retrieve the password from `UserSecrets` in your ASP.NET Core project, add the following code to `Program.cs`:

---

## Summary

- Clean old certificates: `dotnet dev-certs https --clean`
- Export a new certificate: `dotnet dev-certs https --export-path ...`
- Trust the certificate: `dotnet dev-certs https --trust`
- For sensitive values like the certificate password, use `UserSecrets` and store them in `secrets.json`.

This ensures secure storage and easy access to development secrets in ASP.NET Core applications.