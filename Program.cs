using Dotnet2Plantuml;
using Microsoft.CodeAnalysis;

var parser = new Parser();
List<string> classes = new List<string>();
List<string> files = new List<string>();
string? output = null;

var arguments = Environment.GetCommandLineArgs();
for (int i = 1; i < arguments.Count(); i++)
{
    var argument = arguments[i];
    if (argument == "--classes" || argument == "-c")
    {
        i++;
        classes.Add(arguments[i]);
    }
    else if (argument == "--output" || argument == "-o")
    {
        i++;
        output = arguments[i];
    }
    else
    {
        files.Add(argument);
    }
}

foreach(var fileName in files)
{
    Console.WriteLine($"' {fileName}");
    parser.ParseFile(fileName);
}

Graph graph = parser.CurrentGraph;
string text = classes.Any() ? graph.AsText(classes) : graph.AsText();

if (output is null)
{
    Console.WriteLine(text);
}
else
{
    await File.WriteAllTextAsync(output, text);
    Console.WriteLine($"O conteúdo foi escrito no arquivo:\n{output}");
}
