using System;
using System.Reflection;
using System.Threading.Tasks;
using Octokit;

namespace Tsukuru.Services;

internal class AppUpdateProvider : IAppUpdateProvider
{
    private const long _repositoryId = 40837172;
    
    public Version AppVersion { get; }

    public AppUpdateProvider()
    {
        AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }

    public async Task<Release> CheckAsync()
    {
        var client = new GitHubClient(ProductHeaderValue.Parse($"Tsukuru/{AppVersion}"));

        var release = await client.Repository.Release.GetLatest(_repositoryId);

        Version latestVersion;

        if (!Version.TryParse(release.TagName, out latestVersion))
        {
            return null;
        }

        if (latestVersion <= AppVersion)
        {
            return null;
        }

        return release;
    }
}