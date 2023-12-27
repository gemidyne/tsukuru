using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chiaki;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Packer;

public class TemplatingEngine : IDisposable
{
    private const long _maxFileSizeLimitMB = 2097152;
    private readonly List<string> _filesToCleanup;
    private readonly ResultsLogContainer _log;
    private readonly IReadOnlyList<string> _directoriesToScan;

    private readonly string _tokenMapName;

    public TemplatingEngine(
        ResultsLogContainer log,
        IReadOnlyList<string> directoriesToScan,
        string mapName)
    {
        _log = log;
        _directoriesToScan = directoriesToScan;
        _filesToCleanup = new List<string>();

        // For tokens
        _tokenMapName = mapName;
    }

    public void Generate()
    {
        _log.AppendLine(nameof(TemplatingEngine), $"{_directoriesToScan.Count} directories to be processed.");

        foreach (string directory in _directoriesToScan)
        {
            var directoryInfo = new DirectoryInfo(directory);

            var files = directoryInfo.GetFiles("*.tsutmpl", SearchOption.AllDirectories);

            _log.AppendLine(nameof(TemplatingEngine), $"{files.Length} template file(s) found in {directoryInfo.Name}");

            foreach (var file in files)
            {
                if (file.Length >= _maxFileSizeLimitMB)
                {
                    // Don't process files larger than 2MB.
                    _log.AppendLine(nameof(TemplatingEngine), $"WARNING: Cannot process {file.FullName} - file is larger than 2MB limit.");
                    continue;
                }

                // Determine preprocessed filename
                string destinationFileName = PerformReplacements(file.FullName).TrimEnd(".tsutmpl");

                try
                {
                    var lines = File.ReadAllLines(file.FullName);
                    var builder = new StringBuilder();

                    foreach (string line in lines)
                    {
                        builder.AppendLine(PerformReplacements(line));
                    }

                    File.WriteAllText(destinationFileName, builder.ToString());

                    _log.AppendLine(nameof(TemplatingEngine), $"Processed {file.Name} successfully");
                }
                catch (Exception ex)
                {
                    _log.AppendLine(nameof(TemplatingEngine), $"ERROR: Processing {file.FullName} resulted in error: \n{ex}");
                }
                finally
                {
                    _filesToCleanup.Add(destinationFileName);
                }
            }
        }
    }

    public void Dispose()
    {
        int count = 0;

        foreach (var file in _filesToCleanup)
        {
            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                _log.AppendLine(nameof(TemplatingEngine), $"WARNING: Attempted to cleanup {fileInfo.FullName} but it no longer exists...");
                continue;
            }

            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                _log.AppendLine(nameof(TemplatingEngine), $"ERROR: Attempted to cleanup {fileInfo.FullName} resulted in error: \n{ex}");
                continue;
            }

            count++;
        }

        _log.AppendLine(nameof(TemplatingEngine), $"Cleaned up {count} processed file(s)");
    }

    private string PerformReplacements(string input)
    {
        return input
            .Replace("{{map_name}}", _tokenMapName)
            .Replace("{{date}}", DateTime.Now.ToString("yyyy-MM-dd"))
            .Replace("{{time}}", DateTime.Now.ToString("HHmm"));
    }
}