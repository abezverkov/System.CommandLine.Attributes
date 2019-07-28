using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.CommandLine.Attributes
{
    internal static class MethodInfoCommandHelper
    {
        internal static Command CreateCommand(this MethodInfo mi, object instance = null)
        {
            var attr = GetCommandAttribute(mi) ?? CreateCommandAttribute(mi);
            attr.Description = attr.Description ?? CalculateDescription(mi);

            IReadOnlyCollection<Symbol> children = null;
            Argument argument = null;
            Invocation.ICommandHandler handler = CreateCommandHandler(mi);

            return new Command(
                attr.Name
                ,attr.Description
                ,children
                ,argument
                ,attr.TreatUnmatchedTokensAsErrors
                ,handler
                ,attr.IsHidden);
        }

        internal static CommandAttribute GetCommandAttribute(this MethodInfo mi)
        {
            return mi.GetCustomAttributes<CommandAttribute>()
                .FirstOrDefault();
        }

        internal static CommandAttribute CreateCommandAttribute(this MethodInfo mi)
        {
            return new CommandAttribute(mi.Name)
            {
                TreatUnmatchedTokensAsErrors = false,
            };
        }

        internal static string CalculateDescription(this MethodInfo mi)
        {
            string desc;

            desc = GetCommandAttribute(mi)?.Description;
            if (!string.IsNullOrWhiteSpace(desc))
                return desc;

            desc = mi.GetCustomAttributes<DescriptionAttribute>()
                .Select(a => a.Description)
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(desc))
                return desc;

            return null;
        }

        internal static Invocation.ICommandHandler CreateCommandHandler(this MethodInfo mi)
        {
            return Invocation.CommandHandler.Create(mi);
        }
    }
}
