using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace WordListGenerator;

[Generator]
public class WordListGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(string, string)> fileContents = context.AdditionalTextsProvider
            .Where(x => x.Path.EndsWith("Geiriadur.txt", StringComparison.OrdinalIgnoreCase))
            .Select((a, c) => (Path.GetFileNameWithoutExtension(a.Path), a.GetText(c)!.ToString()));

        IncrementalValuesProvider<ImmutableArray<string>> linesProvider = fileContents.Select((fileContent, _) =>
            fileContent.Item2.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToImmutableArray());

        context.RegisterSourceOutput(linesProvider, (ctx, lines) =>
        {
            ctx.AddSource("WordList.g.cs",
                $$"""
                  using System.Collections.Immutable;
                  namespace Cyfrinair.Core
                  {
                      public static class WordList
                      {
                          public static readonly ImmutableArray<string> Words = ImmutableArray.Create({{string.Join(", ", lines.Select(x => $@"""{x}"""))}});
                      }
                  }
                  """);
        });
    }
}