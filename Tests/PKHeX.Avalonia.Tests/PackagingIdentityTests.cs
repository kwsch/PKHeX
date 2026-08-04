namespace PKHeX.Avalonia.Tests;

public class PackagingIdentityTests
{
    [Fact]
    public void WindowsPackaging_UsesPublicApplicationIdentity()
    {
        var installer = ReadRepositoryFile("packaging", "windows", "installer.iss");
        var wingetLocale = ReadRepositoryFile("packaging", "winget", "realgarit.PKHeXAvalonia.locale.en-US.yaml");
        var wingetVersion = ReadRepositoryFile("packaging", "winget", "realgarit.PKHeXAvalonia.yaml");
        var wingetInstaller = ReadRepositoryFile("packaging", "winget", "realgarit.PKHeXAvalonia.installer.yaml");
        var buildProperties = ReadRepositoryFile("Directory.Build.props");
        var applicationProject = ReadRepositoryFile("PKHeX.Avalonia", "PKHeX.Avalonia.csproj");
        var signingCertificateScript = ReadRepositoryFile("Scripts", "make-signing-cert.sh");

        AssertHasLine(installer, "#define MyAppName \"PKHeX-Avalonia\"");
        AssertHasLine(installer, "#define MyAppPublisher \"Patrik Lleshaj\"");
        AssertHasLine(installer, "#define MyAppURL \"https://github.com/realgarit/PKHeX-Avalonia\"");
        AssertHasLine(installer, "AppId={{B6C9F1B4-7B7B-4B7A-9C2E-8B6C9C7B7B7B}}");
        AssertHasLine(installer, "AppVerName={#MyAppName}");
        AssertHasLine(installer, "UninstallDisplayName={#MyAppName}");

        foreach (var manifest in new[] { wingetLocale, wingetVersion, wingetInstaller })
            AssertHasLine(manifest, "PackageIdentifier: realgarit.PKHeXAvalonia");

        AssertHasLine(wingetLocale, "Publisher: Patrik Lleshaj");
        AssertHasLine(wingetLocale, "PublisherUrl: https://github.com/realgarit");
        AssertHasLine(wingetLocale, "PublisherSupportUrl: https://github.com/realgarit/PKHeX-Avalonia/issues");
        AssertHasLine(wingetLocale, "PackageName: PKHeX-Avalonia");
        AssertHasLine(wingetLocale, "PackageUrl: https://github.com/realgarit/PKHeX-Avalonia");
        AssertHasLine(wingetLocale, "LicenseUrl: https://github.com/realgarit/PKHeX-Avalonia/blob/main/LICENSE");
        AssertHasLine(wingetInstaller, "InstallerUrl: https://github.com/realgarit/PKHeX-Avalonia/releases/download/v{{VERSION}}/PKHeX-Avalonia-Setup.exe");

        AssertHasLine(applicationProject, "<Product>PKHeX-Avalonia</Product>");
        AssertHasLine(applicationProject, "<AssemblyTitle>PKHeX-Avalonia</AssemblyTitle>");
        AssertHasLine(buildProperties, "<Company>Patrik Lleshaj</Company>");
        AssertHasLine(buildProperties, "<Authors>Patrik Lleshaj</Authors>");
        AssertHasLine(buildProperties, "<Copyright>Copyright © Patrik Lleshaj</Copyright>");

        AssertHasLine(signingCertificateScript, "# Env:     P12_PASSWORD (required)   CERT_CN (default \"Patrik Lleshaj\")");
        AssertHasLine(signingCertificateScript, "CN=\"${CERT_CN:-Patrik Lleshaj}\"");
    }

    private static void AssertHasLine(string content, string expected) =>
        Assert.Contains(expected, content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()));

    private static string ReadRepositoryFile(params string[] relativePath)
    {
        var path = Path.Combine([FindRepoRoot(), .. relativePath]);
        Assert.True(File.Exists(path), $"Packaging file not found: {path}");
        return File.ReadAllText(path);
    }

    private static string FindRepoRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (!Directory.Exists(Path.Combine(dir, "packaging")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null)
                throw new DirectoryNotFoundException("Could not find repository root");
            dir = parent.FullName;
        }

        return dir;
    }
}
