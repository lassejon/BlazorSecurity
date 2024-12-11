namespace BlazorSecurity.Startup;

using System;
using System.IO;
using System.Security.AccessControl;

public static class FileSecurityManager
{
    public static void SetFilePermissions(string filePath)
    {
        if (OperatingSystem.IsWindows())
        {
            var fileInfo = new FileInfo(filePath);
            var security = fileInfo.GetAccessControl();

            security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
            var rule = new FileSystemAccessRule(
                Environment.UserName,
                FileSystemRights.FullControl,
                AccessControlType.Allow
            );

            security.AddAccessRule(rule);
            fileInfo.SetAccessControl(security);
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            var chmodCommand = $"chmod 600 {filePath}";
            System.Diagnostics.Process.Start("bash", $"-c \"{chmodCommand}\"");
        }
    }
}
