using System.Text;
using System.Text.RegularExpressions;


public static class MarkdownTable
{
    public static async Task GenerateTable(string distFolder)
    {
        Console.WriteLine("Generating Markdown table...");

        var sbTable = new StringBuilder();
        sbTable.AppendLine("Category | Macro (Name) | <pre>Color</pre> | <pre>Mono </pre> | Url");
        sbTable.AppendLine("  ---    |  ---  | :---:  | :---: | ---");

        var currentCategory = "";

        // Sort by category, then name, so the table is stable between runs
        var filePaths = Directory.GetFiles(distFolder, "*.puml", SearchOption.AllDirectories)
            .Where(p => Directory.GetParent(p)!.FullName != Path.GetFullPath(distFolder))
            .OrderBy(p => Directory.GetParent(p)!.Name, StringComparer.Ordinal)
            .ThenBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal);

        foreach (var filePath in filePaths)
        {
            var entityName = Path.GetFileNameWithoutExtension(filePath);
            var category = Directory.GetParent(filePath)!.Name;

            if (entityName == "all")
            {
                continue;
            }
            if(currentCategory != category)
            {
                sbTable.AppendLine($"**{category}** | | | | **{category}/all.puml**");
                currentCategory = category;
            }

            sbTable.Append($"{category} | ");
            sbTable.Append($"{entityName} </br> ({Regex.Replace(entityName, "(?<!^)(([A-Z][a-z])|([A-Z][A-Z0-9][A-Z])|([A-Z][A-Z]?[a-z][A-Z]))", " $1")}) | ");

            if(File.Exists(Path.Combine(distFolder, $"{category}/{entityName}.png")))
            {
                sbTable.Append($"![{entityName}](dist/{category}/{entityName}.png?raw=true) | ");
            }  
            else 
            {
                sbTable.Append($" |");
            }
            
            sbTable.Append($"![{entityName}](dist/{category}/{entityName}(m).png?raw=true) | ");
            sbTable.AppendLine($"{category}/{entityName}.puml");
        }

        await File.WriteAllTextAsync(Path.Combine(distFolder, "table.md"), sbTable.ToString());
    }

}

