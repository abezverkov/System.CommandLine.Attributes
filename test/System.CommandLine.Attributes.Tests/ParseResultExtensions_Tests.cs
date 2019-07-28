using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace System.CommandLine.Attributes.Tests
{
    public class ParseResultExtensions_Tests
    {
        private ParseResult _results;

        public ParseResultExtensions_Tests()
        {
            var command = new Command("test")
            {
                new Option("--user",  "", new Argument<string>())
            };

            var args = new[] { "--user=bob" };
            _results = command.Parse(args);
        }

        [Fact]
        public void TestMethod1()
        {
            var results = _results.OptionsDictionary();

            results.Should().NotBeNull();
            results.ContainsKey("--user").Should().BeTrue();
            results["--user"].Should().Be("bob");
        }

        [Fact]
        public void MyTestMethod2()
        {
            var results = _results.OptionsCommandLineArgs();

            results.Should().NotBeNull();
            results.Should().HaveCount(1);
            results[0].Should().Be("--user=bob");
        }

        [Fact]
        public void MyTestMethod3()
        {
            var config = new ConfigurationBuilder()
                .AddSystemCommandLineOptions(_results)
                .Build();

            config["user"].Should().Be("bob");
            config["foo"].Should().BeNull();
        }
    }
}
