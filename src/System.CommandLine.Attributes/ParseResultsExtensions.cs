using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace System.CommandLine.Attributes
{
    public static class ParseResultsExtensions
    {
        public static Dictionary<string, object> OptionsDictionary(this ParseResult results)
        {
            var command = results.CommandResult;
            return results
                .Tokens
                .Where(t => t.Type == TokenType.Option)
                .Select(o => new { key = o.Value, value = command.ValueForOption(o.Value) })
                .ToDictionary(o => o.key, o => o.value);
        }

        public static string[] OptionsCommandLineArgs(this ParseResult results, char separator = '=')
        {
            var command = results.CommandResult;
            var options = results.Tokens
                .Where(t => t.Type == TokenType.Option);

            var args = new List<string>();
            foreach( var o in options)
            {
                var value = command.ValueForOption(o.Value);
                if (!(value is string) && (value is IEnumerable i))
                {
                    foreach (var v in i) args.Add($"{o.Value}{separator}{v}");
                }
                else
                    args.Add($"{o.Value}{separator}{value}");
            }
            return args.ToArray();
        }

        public static IConfigurationBuilder AddSystemCommandLineOptions(this IConfigurationBuilder builder, ParseResult results)
        {
            var args = results.OptionsCommandLineArgs();
            builder.AddCommandLine(args);
            return builder;
        }
    }
}
