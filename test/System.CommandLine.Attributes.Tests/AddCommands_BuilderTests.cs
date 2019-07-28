using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.ComponentModel;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Builder;

namespace System.CommandLine.Attributes.Tests
{
    public class When_Testing_Adding_Commands_FromBuilder_WithOptions_From_Type
    {

        public interface ITestService
        {
            string DoSomething();
        }


        public class TestService : ITestService
        {
            public string DoSomething() { return "Hello World"; }
        }
    
        public class TestOptions
        {
            [DefaultValue("foo")]
            public string MyProperty1 { get; set; } = "foo";
            public int MyProperty2 { get; set; }
        }
        public class TestCommands
        {
            private readonly ITestService _service;

            public TestCommands(ITestService service)
            {
                this._service = service ?? throw new ArgumentNullException(nameof(service));
            }

            public void TestMethod1(IConsole conn) { conn.Out.Write($"TestServices.DoSomething={_service.DoSomething()}"); }
            public void TestMethod2(IConsole conn, string MyProperty1) { conn.Out.Write($"MyProperty1={MyProperty1}"); }

            private void DummyMethod1() { }
            protected void DummyMethod2() { }
            internal void DummyMethod3() { }
        }

        private CommandLineBuilder _builder;

        public When_Testing_Adding_Commands_FromBuilder_WithOptions_From_Type()
        {
            _builder = new CommandLineBuilder()
                .AddCommands<TestCommands, TestOptions>();
        }

        [Fact]
        public void Given_A_Class_TestCommands()
        {
            _builder.Build().GetSubCommands()
                .Should().HaveCount(2);
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod1()
        {
            var command = _builder.Build()
                .GetSubCommand<TestCommands>(x => nameof(x.TestMethod1));

            command.Handler.Should().NotBeNull();

            var args = new[] { "TestMethod1" };
            var presult = command.Parse(args);

            var services = new ServiceCollection();
            services.AddTransient<ITestService, TestService>();


            var con = new TestConsole();

            var result = _builder
                //.UseDependencyInjection(ctx =>
                //{
                //    ctx.AddService(typeof(ITestService), () => new TestService());
                //})
                .UseDependencyInjection(services)
                .UseExceptionHandler((ex,ctx) =>
                {
                    con.Error.Write(ex.ToString());
                })
                .Build()
                .InvokeAsync(args, con);


            con.Error.ToString().Should().Be(string.Empty);
            con.Out.ToString().Should().Be("TestServices.DoSomething=Hello World");
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod1b()
        {
            var command = _builder.Build()                
                .GetSubCommand<TestCommands>(x => nameof(x.TestMethod1));

            command.Handler.Should().NotBeNull();

            var args = new[] { "TestMethod1" };
            var presult = command.Parse(args);

            var con = new TestConsole();

            var result = _builder
                .UseDependencyInjection(services =>
                {
                    services.AddTransient<ITestService, TestService>();
                })
                .UseExceptionHandler((ex, ctx) =>
                {
                    con.Error.Write(ex.ToString());
                })
                .Build()
                .InvokeAsync(args, con);


            con.Error.ToString().Should().Be(string.Empty);
            con.Out.ToString().Should().Be("TestServices.DoSomething=Hello World");
        }


        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod2()
        {
            var command = _builder.Build().
                GetSubCommand<TestCommands>(x => nameof(x.TestMethod2));

            command.Handler.Should().NotBeNull();

            var args = new[] { "TestMethod2" };
            var presult = command.Parse(args);

            var con = new TestConsole();

            var result = _builder
                .UseBindingContextInjection(ctx =>
                {
                    ctx.AddService(typeof(ITestService), () => new TestService());
                })
                .UseExceptionHandler((ex, ctx) =>
                {
                    con.Error.Write(ex.ToString());
                })
                .Build()
                .InvokeAsync(args, con);


            con.Error.ToString().Should().Be(string.Empty);
            con.Out.ToString().Should().Be("MyProperty1=foo");
        }

        [Fact]
        public void Given_TestCommands_Has_Properties()
        {
            var command = _builder.Build()
                .GetSubCommand<TestCommands>(x => nameof(x.TestMethod1));

            command.Children.OfType<Option>().Should().HaveCount(2);
        }

        [Fact]
        public void MyTestMethod()
        {
            var command = _builder.Build()
                .GetSubCommand<TestCommands>(x => nameof(x.TestMethod1));
            ISymbol symbol = command.GetOption<TestOptions>(x => x.MyProperty1);

            // Parser required that there be an alias that matching symbol.Name
            // else, Will file with 'Sequence contains no elements
            // This  is the test to make sure we are adding it.
            symbol.RawAliases.First(alias => alias.Replace("--", "") == symbol.Name);
        }
    }

    public class When_Testing_Adding_Commands_From_Type_With_Builder
    {
        public class TestCommands
        {
            private readonly IConsole _conn;

            public TestCommands(IConsole conn)
            {
                this._conn = conn;
            }

            public void TestMethod1() { }

            [Description("this is test method #2")]
            public void TestMethod2() { }

            [Command("TestMethod3", Description = "this is test method #3")]
            public void TestMethod3(IConsole conn) { conn.Out.Write("Write TestMethod 3"); }

            [Command("Baz")]
            public void TestMethod4(string x) { _conn.Out.Write($"Write TestMethod 4{x}"); }

            private void DummyMethod1() { }
            protected void DummyMethod2() { }
            internal void DummyMethod3() { }
        }

        private CommandLineBuilder _builder;

        public When_Testing_Adding_Commands_From_Type_With_Builder()
        {
            _builder = new CommandLineBuilder()
                .AddCommands<TestCommands>();
        }

        [Fact]
        public void Given_A_Class_TestCommands()
        {
            _builder.Build().GetSubCommands()
                .Should().HaveCount(4);
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod1()
        {
            var command = _builder.Build().GetSubCommand<TestCommands>(x => nameof(x.TestMethod1));

            command.Description.Should().Be(null);
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod2()
        {
            var command = _builder.Build().GetSubCommand<TestCommands>(x => nameof(x.TestMethod2));

            command.Description.Should().Be("this is test method #2");
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod3()
        {
            var command = _builder.Build().GetSubCommand<TestCommands>(x => nameof(x.TestMethod3)) as Command;

            command.Description.Should().Be("this is test method #3");
            command.Handler.Should().NotBeNull();

            var args = new[] { "Baz" };
            var con = new TestConsole();
            var result = command.InvokeAsync(args, con).Result;

            con.Error.ToString().Should().Be(string.Empty);
            con.Out.ToString().Should().Be("Write TestMethod 3");
        }

        [Fact]
        public void Given_TestCommands_Has_Method_TestMethod4()
        {
            var command = _builder.Build().GetSubCommand<TestCommands>(x => nameof(x.TestMethod4)) as Command;
            command.Should().BeNull();

            command = _builder.Build().GetSubCommand("Baz") as Command;
            command.Description.Should().Be(null);

            command.Handler.Should().NotBeNull();

            var args = new[] { "Baz" };
            var con = new TestConsole();
            var result = command.InvokeAsync(args, con).Result;

            con.Error.ToString().Should().Be(string.Empty);
            con.Out.ToString().Should().Be("Write TestMethod 4");
        }
    }
}
