using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class OptionAliasAttribute : Attribute
    {
        public OptionAliasAttribute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentException("message", nameof(alias));
            }

            Alias = alias;
        }

        public string Alias { get; }
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(string alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public string Alias { get; }
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(string name, ArityEnum arity = ArityEnum.Unknown)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Arity = arity;
        }

        public string Name { get; }
        public ArityEnum Arity { get; set; }

        public string Description { get; set; }
        public object DefaultValue { get; set; }
        internal Type Type { get; set; }
    }

    
    public enum ArityEnum: int
    {
        Unknown,    // null
        Zero,       // Zero => new ArgumentArity(0, 0);
        ZeroOrOne,  // ZeroOrOne => new ArgumentArity(0, 1);
        ExactlyOne, // ExactlyOne => new ArgumentArity(1, 1);
        ZeroOrMore, // ZeroOrMore => new ArgumentArity(0, 2147483647);
        OneOrMore, 	// OneOrMore => new ArgumentArity(1, 2147483647);
    }

}
