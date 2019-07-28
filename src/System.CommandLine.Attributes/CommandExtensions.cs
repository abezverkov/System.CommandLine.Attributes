using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.CommandLine.Builder;

namespace System.CommandLine.Attributes
{
    public static class CommandExtensions
    {        public static IEnumerable<Option> GetOptions(this Command command)
        {
            return command.Children.OfType<Option>();
        }

        public static Option GetOption(this Command command, string name)
        {
            return command.GetOptions().SingleOrDefault(c => c.Name == name);
        }

        public static Option GetOption<TOptions>(this Command command, Expression<Func<TOptions, object>> expression)
        {
            var name = expression.GetMemberName();
            return command.GetOption(name);
        }

        public static Command AddOptions<TOptions>(this Command command) where TOptions : class
        {
            typeof(TOptions)
                .GetProperties()
                .Select(pi => pi.CreateOption())
                .ToList()
                .ForEach(o => command.AddOption(o));
            return command;
        }

        public static IEnumerable<Command> GetSubCommands(this Command command)
        {
            return command.Children.OfType<Command>();
        }

        public static IEnumerable<Command> GetSubCommands(this Parser parser)
        {
            var rootCommand = (parser.Configuration.RootCommand as Command);
            return rootCommand?.GetSubCommands();
        }

        public static Command GetSubCommand(this Command command, string name)
        {
            return command.GetSubCommands().SingleOrDefault(c => c.Name == name);
        }


        public static ICommand GetSubCommand(this Parser parser, string name)
        {
            var rootCommand = (parser.Configuration.RootCommand as Command);
            return rootCommand.GetSubCommands().SingleOrDefault(c => c.Name == name);
        }

        public static Command GetSubCommand<TCommands>(this Command command, Expression<Func<TCommands, object>> expression)
        {
            var name = expression.GetMemberName();
            return command.GetSubCommand(name);
        }

        public static Command GetSubCommand<TCommands>(this Parser parser, Expression<Func<TCommands, object>> expression)
        {
            var name = expression.GetMemberName();
            var rootCommand = (parser.Configuration.RootCommand as Command);
            return rootCommand.GetSubCommand(name);
        }

        public static Command AddCommands<TCommands>(this Command command) where TCommands : class
        {
            typeof(TCommands)
                .GetMethodsOnly()
                .Select(mi => mi.CreateCommand())
                .ToList()
                .ForEach(c => command.AddCommand(c));
            return command;
        }

        public static CommandLineBuilder AddCommands<TCommands>(this CommandLineBuilder builder) where TCommands : class
        {
            typeof(TCommands)
                .GetMethodsOnly()
                .Select(mi => mi.CreateCommand())
                .ToList()
                .ForEach(c => builder.AddCommand(c));
            return builder;
        }

        public static Command AddCommands<TCommands, TOptions>(this Command command) 
            where TCommands : class
            where TOptions : class
        {
            typeof(TCommands)
                .GetMethodsOnly()
                .Select(mi => mi.CreateCommand().AddOptions<TOptions>())
                .ToList()
                .ForEach(c => command.AddCommand(c));
            return command;
        }

        public static CommandLineBuilder AddCommands<TCommands, TOptions>(this CommandLineBuilder builder)
            where TCommands : class
            where TOptions : class
        {
            typeof(TCommands)
                .GetMethodsOnly()
                .Select(mi => mi.CreateCommand().AddOptions<TOptions>())
                .ToList()
                .ForEach(c => builder.AddCommand(c));
            return builder;
        }

        private static IEnumerable<MethodInfo> GetMethodsOnly(this Type t, BindingFlags? bindingAttr = null)
        {
            var default_flags = (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            return t.GetMethods(bindingAttr ?? default_flags)
                .Where(mi => !mi.IsSpecialName);
        }
    }
}
