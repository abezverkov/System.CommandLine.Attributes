using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.CommandLine.Attributes
{
    internal static class PropertyInfoOptionHelper
    {
        internal static Option CreateOption(this PropertyInfo pi)
        {
            var optAttr = GetOptionAttribute(pi);
            var opt = new Option(
                CalculateAlias(pi),
                CalculateDescription(pi),
                CreateArgument(pi),
                optAttr?.IsHidden ?? false);
            opt.Name = pi.Name;
            return opt;
        }

        internal static string[] CalculateAlias(this PropertyInfo pi)
        {
            var aliases = new List<string>();
            aliases.Add(GetOptionAttribute(pi)?.Alias?.ToLower());
            aliases.AddRange(pi
                .GetCustomAttributes<OptionAliasAttribute>()
                .Select(a => a.Alias?.ToLower())
            );
            aliases = aliases
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct().ToList();

            if (!aliases.Any())
            {
                aliases.Add(pi.Name.ToLower());
            }
            if (!aliases.Contains(pi.Name))
            {
                aliases.Add(pi.Name);
            }

            return aliases
                .Select(a => a.StartsWith("--") ? a : $"--{a}")
                .ToArray();
        }

        internal static string CalculateDescription(this PropertyInfo pi)
        {
            string desc;

            desc = GetOptionAttribute(pi)?.Description;
            if (!string.IsNullOrWhiteSpace(desc))
                return desc;

            desc = pi.GetCustomAttributes<DescriptionAttribute>()
                .Select(a => a.Description)
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(desc))
                return desc;

            return pi.Name;
        }

        internal static OptionAttribute GetOptionAttribute(this PropertyInfo pi)
        {
            return pi.GetCustomAttributes<OptionAttribute>()
                .FirstOrDefault();
        }

        internal static Argument CreateArgument(this PropertyInfo pi)
        {
            var attr = GetArgumentAttribute(pi)
                ?? CreateArgumentAttribute(pi);
            if (attr.Arity == ArityEnum.Unknown)
                attr.Arity = CalculateArity(pi);

            var arg = new Argument()
            {
                Name = attr.Name,
                Description = attr.Description,
                ArgumentType = attr.Type,
                Arity = TranslateArity(attr.Arity),
            };
            if (attr.DefaultValue != null)
                arg.SetDefaultValue(attr.DefaultValue);
            else if (attr.Type == typeof(bool))
                arg.SetDefaultValue(false);

            return arg;
        }

        internal static ArgumentAttribute GetArgumentAttribute(this PropertyInfo pi)
        {
            var attr = pi.GetCustomAttributes<ArgumentAttribute>()
                .FirstOrDefault();
            if (attr != null)
            {
                attr.Type = CalculateArgumentType(pi);
            }
            return attr;
        }

        internal static ArgumentAttribute CreateArgumentAttribute(this PropertyInfo pi)
        {
            string desc = null;
            var attr = new ArgumentAttribute(pi.Name)
            {
                Description = desc,
                Type = CalculateArgumentType(pi),
                DefaultValue = CalculateArgumentDefaultValue(pi)
            };
            return attr;
        }

        internal static ArityEnum CalculateArity(this PropertyInfo pi)
        {
            if (pi.PropertyType.IsEnumerable())
                return ArityEnum.ZeroOrMore;

            if (pi.PropertyType == typeof(bool))
                return ArityEnum.ZeroOrOne;

            return ArityEnum.ZeroOrOne;
        }

        internal static IArgumentArity TranslateArity(ArityEnum arity)
        {
            switch (arity)
            {
                case ArityEnum.Zero: return ArgumentArity.Zero; // => new ArgumentArity(0, 0);
                case ArityEnum.ZeroOrOne: return ArgumentArity.ZeroOrOne; // => new ArgumentArity(0, 1);
                case ArityEnum.ExactlyOne: return ArgumentArity.ExactlyOne; // => new ArgumentArity(1, 1);
                case ArityEnum.ZeroOrMore: return ArgumentArity.ZeroOrMore; // => new ArgumentArity(0, 2147483647);
                case ArityEnum.OneOrMore: return ArgumentArity.OneOrMore; // => new ArgumentArity(1, 2147483647);
                default:
                    throw new ArgumentOutOfRangeException(nameof(arity));
            }
        }

        internal static object CalculateArgumentDefaultValue(this PropertyInfo pi)
        {
            object value;

            value = GetArgumentAttribute(pi)?.DefaultValue;
            if (value != null)
                return value;

            value = pi.GetCustomAttributes<DefaultValueAttribute>()
                .Select(a => a.Value)
                .FirstOrDefault();
            if (value != null)
                return value;

            return value;
        }

        internal static Type CalculateArgumentType(this PropertyInfo pi)
        {
            return pi.PropertyType.GetEnumerableType() ?? pi.PropertyType;
        }
    }
}
