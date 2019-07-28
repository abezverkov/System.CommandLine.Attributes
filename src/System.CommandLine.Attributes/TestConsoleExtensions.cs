using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.CommandLine.Attributes
{
    public static class TestConsoleExtensions
    {
        public static TestConsole RedirectSystemConsole(this TestConsole console)
        {
            System.Console.SetOut((TextWriter)console.Out);
            System.Console.SetError((TextWriter)console.Error);
            return console;
        }

        public static TestConsole ResetSystemConsole(this TestConsole console)
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);

            var standardError = new StreamWriter(Console.OpenStandardError());
            standardError.AutoFlush = true;
            Console.SetError(standardError);

            return console;
        }
    }
}
