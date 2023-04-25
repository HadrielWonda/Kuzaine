namespace Kuzaine.Services;

using System.Reflection;
using Spectre.Console;

public static class VersionChecker
{
    public static async Task CheckForLatestVersion()
    {
        try
        {
            var installedVersion = GetInstalledKuzaineVersion();
            var latestReleaseVersion = await GetLatestReleaseVersion();
            var result = new Version(installedVersion).CompareTo(new Version(latestReleaseVersion));
            if (result < 0)
            {
                AnsiConsole.MarkupLine(@$"{Environment.NewLine}[bold seagreen2]This Kuzaine version '{installedVersion}' is older than that of the runtime '{latestReleaseVersion}'. Update the tools for the latest features and bug fixes (`dotnet tool update -g Kuzaine`).[/]{Environment.NewLine}");
            }
        }
        catch (Exception)
        {
            // fail silently
        }
    }
    
    private static async Task<string> GetLatestReleaseVersion()
    {
        var latestKuzainePath = "https://github.com/pdevito3/Kuzaine/releases/latest";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "text/html");

        var response = await client.GetAsync(latestKuzainePath);
        response.EnsureSuccessStatusCode();

        var redirectUrl = response?.RequestMessage?.RequestUri;
        string version = redirectUrl?.ToString().Split('/').Last().Replace("v", "");
        return version;
    }

    private static string GetInstalledKuzaineVersion()
    {
        var installedVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        installedVersion = installedVersion[0..^2]; // equivalent to installedVersion.Substring(0, installedVersion.Length - 2);

        return installedVersion;
    }
}