using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

namespace WordListGenerator
{
    [Generator]
    public class WordListGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var fileContents = context.AdditionalTextsProvider
                .Where(x => x.Path.EndsWith("cy_GB.dic", StringComparison.OrdinalIgnoreCase))
                .Select((a, c) => (Path.GetFileNameWithoutExtension(a.Path), a.GetText(c)!.ToString()));

            var processedLinesProvider = fileContents.Select((fileContent, _) =>
            {
                var processedLines = new List<string>();
                foreach (var line in fileContent.Item2.Split(new[] { '\r', '\n' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    var processedLine = ProcessLine(line);
                    if (!string.IsNullOrWhiteSpace(processedLine))
                    {
                        processedLines.Add(processedLine);
                    }
                    
                }
                return processedLines.ToImmutableArray();
            });

            context.RegisterSourceOutput(processedLinesProvider, (ctx, wordList) =>
            {
                ctx.AddSource("WordList.g.cs", 
                    $$"""
                      using System.Collections.Immutable;
                      namespace Cyfrinair.Core
                      {
                          public static class WordList 
                          {
                              public static readonly ImmutableArray<string> Words = ImmutableArray.Create({{string.Join(", ", wordList.Select(x => $@"""{x}"""))}});
                          }
                      }
                      """);
            });
        }

        private static string ProcessLine(string line)
        {
            // Skip lines that start with a digit or are empty
            if(char.IsDigit(line[0]) | string.IsNullOrWhiteSpace(line))
            {
                return string.Empty;
            }
            
            // Assume we have a word and split it to discard the encoding after the slash
            var word = line.Split('/')[0].Trim();
            
            // Skip lines containing punctuation
            if(word.Contains('\'') | word.Contains('.') | word.Contains('-'))
            {
                return string.Empty;
            }
            // Skip lines with words that are too short
            if (word.Length < 4)
            {
                return string.Empty;
            }

            if (word.Length > 10)
            {
                return string.Empty;
            }

            // Skip lines starting with a capital letter as we assume this a place name or similar
            if (char.IsUpper(word[0]))
            {
                return string.Empty;
            }
            
            word = RemoveDiacritics(word);
            
            return word;
        }

        private static string RemoveDiacritics(string word)
        {
            var normalizedString = word.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
            
        }
    }
}