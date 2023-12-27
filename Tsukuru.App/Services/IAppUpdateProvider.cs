using System;
using System.Threading.Tasks;
using Octokit;

namespace Tsukuru.Services;

public interface IAppUpdateProvider
{
    Version AppVersion { get; }
    
    Task<Release> CheckAsync();
}