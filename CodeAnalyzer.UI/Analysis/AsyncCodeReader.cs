using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Analysis;

internal sealed class AsyncCodeReader
{
    private static readonly string[] ExcludedFolders = ["obj", "bin", "Tests", "Generated"];
    
    public async Task<IEnumerable<string>> ReadCode(ICodePathProvider codePathProvider)
    {
        string filePath = codePathProvider.ProvideFile();
        if (!string.IsNullOrEmpty(filePath))
        {
            string code = await File.ReadAllTextAsync(filePath);
            return new List<string> { code };
        }
        
        string folderPath = codePathProvider.ProvideFolder();
        if (string.IsNullOrEmpty(folderPath))
        {
            return [];
        }
        
        
        IEnumerable<string> files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
            .Where(path => !ExcludedFolders.Any(folder => path.Split(Path.DirectorySeparatorChar).Contains(folder)));
            
        return await Task.WhenAll(files.Select(file => File.ReadAllTextAsync(file)));
    }
}