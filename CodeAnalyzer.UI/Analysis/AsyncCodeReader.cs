using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Core.Logging.Interfaces;
using CodeAnalyzer.Parser.Dtos;
using CodeAnalyzer.UI.Interfaces;
using CodeAnalyzer.UI.LoggerUi.Builders.OtherEntryBuilders;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class AsyncCodeReader
{
    private static readonly string[] ExcludedFolders = ["obj", "bin", "Tests", "Generated"];
    
    private readonly FileEntryBuilder _fileEntryBuilder = new();
    
    public ILogger? Logger { get; set; }
    
    public async Task<IEnumerable<FileDto>> ReadCode(ICodePathProvider codePathProvider)
    {
        try
        {
            Logger?.OpenLevel("Czytanie plików z kodem");
            
            string filePath = codePathProvider.ProvideFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                Logger?.Info($"Czytanie pliku {filePath}");
                string code = await File.ReadAllTextAsync(filePath);
                
                Logger?.Info($"Pomyślnie przeczytano kod z pliku {filePath}");
                return new List<FileDto> { new(filePath, code) };
            }

            string folderPath = codePathProvider.ProvideFolder();
            if (string.IsNullOrEmpty(folderPath))
            {
                Logger?.Info("Nie znaleziono ani pliku ani folderu do analizy");
                return [];
            }

            Logger?.Info($"Czytanie folderu {folderPath}");
            List<string> files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => !ExcludedFolders.Any(folder => path.Split(Path.DirectorySeparatorChar).Contains(folder)))
                .ToList();

            LogFilePaths(files);
            FileDto[] readFiles = await ReadFilesAsync(files);
            LogFiles(readFiles);
            return readFiles;
        }
        catch (Exception ex)
        {
            Logger?.Exception(ex);
            throw;
        }
        finally
        {
            Logger?.CloseLevel();
        }
    }

    private async Task<FileDto[]> ReadFilesAsync(List<string> files)
    {
        IEnumerable<Task<FileDto>> tasks = files.Select(async file =>
        {
            string content = await File.ReadAllTextAsync(file);
            return new FileDto(file, content);
        });

        return await Task.WhenAll(tasks);
    }

    private void LogFilePaths(List<string> files)
    {
        try
        {
            Logger?.OpenLevel($"[{files.Count}] Znalezione pliki z kodem źródłowym C#");
            files.ForEach(f => Logger?.Info(f));
        }
        finally
        {
            Logger?.CloseLevel();
        }
    }

    private void LogFiles(FileDto[] files)
    {
        try
        {
            Logger?.OpenLevel($"[{files.Length}] Przeczytane pliki");
            files.ToList().ForEach(f => Logger?.Info(_fileEntryBuilder.Build(f).Title));
        }
        finally
        {
            Logger?.CloseLevel();
        }
    }
}