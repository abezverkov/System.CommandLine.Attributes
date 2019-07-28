using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute: Attribute
    {
        public CommandAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public string Description { get; set; }
        public bool TreatUnmatchedTokensAsErrors { get; set; }
        public bool IsHidden { get; set; }
    }
}
