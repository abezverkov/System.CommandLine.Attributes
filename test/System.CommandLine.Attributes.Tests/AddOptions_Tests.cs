using System;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace System.CommandLine.Attributes.Tests
{
    public class When_Adding_Options_By_Type
    {
        public class MyOptions
        {
            public string User { get; set; }
            public string Password { get; set; }
            public bool Switch { get; set; }

            [DefaultValue(true)]
            public bool Switch2 { get; set; }

            [Option("--attr1", Description = "desc1", IsHidden = true)]
            public string WithOptionAttribute { get; set; }

            [OptionAlias("--attr2")]
            [OptionAlias("--attr-2")]
            public string WithOptionAliasAttributes { get; set; }

            [Argument("Colors", Arity = ArityEnum.OneOrMore)]
            public int[] Colors { get; set; }

            [Argument("Flowers")]
            public string[] Flowers { get; set; }

            private object DummyProp1 { get; set; }

            protected object DummyProp2 { get; set; }
            internal object DummyProp3 { get; set; }

            private object DummyField1;
            protected object DummyField2;
            internal object DummyField3;
            public object DummyField4;

            private void DummyMethod1() { }
            internal void DummyMethod2() { }
            public void DummyMethod3() { }
        }

        private Command _command;

        public When_Adding_Options_By_Type()
        {
            _command = new Command("exec", "a description goes here")
                     .AddOptions<MyOptions>();
        }

        [Fact]
        public void Given_Type_Has_Properties()
        {
            _command.GetOptions().Should().HaveCount(8);
        }

        [Fact]
        public void Given_Type_Has_User_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.User);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("user");
            o.Aliases.Should().Contain("User");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given__User_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.User);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<string>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(1);
            a.Description.Should().Be(null);
            a.Name.Should().Be("User");           
            a.Parent.Should().BeSameAs(o);
            a.GetDefaultValue().Should().Be(null);
        }

        [Fact]
        public void Given_Type_Has_Switch_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.Switch);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("switch");
            o.Aliases.Should().Contain("Switch");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_Switch_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.Switch);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<bool>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(1);
            a.Description.Should().Be(null);
            a.Name.Should().Be("Switch");
            a.Parent.Should().BeSameAs(o);
            a.GetDefaultValue().Should().Be(false);
        }

        [Fact]
        public void Given_Type_Has_Switch2_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.Switch2);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("switch2");
            o.Aliases.Should().Contain("Switch2");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_Switch2_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.Switch2);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<bool>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(1);
            a.Description.Should().Be(null);
            a.Name.Should().Be("Switch2");
            a.Parent.Should().BeSameAs(o);
            a.GetDefaultValue().Should().Be(true);
        }

        [Fact]
        public void Given_Type_Has_Attributed_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.WithOptionAttribute);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("attr1");
            o.Aliases.Should().Contain("WithOptionAttribute");
            o.IsHidden.Should().BeTrue();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_Attributed_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.WithOptionAttribute);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<string>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(1);
            a.Description.Should().Be(null);
            a.Name.Should().Be("WithOptionAttribute");
            a.Parent.Should().BeSameAs(o);
        }

        [Fact]
        public void Given_Type_Has_AliasAttributed_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.WithOptionAliasAttributes);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(3);
            o.Aliases.Should().Contain("WithOptionAliasAttributes");
            o.Aliases.Should().Contain("attr-2");
            o.Aliases.Should().Contain("attr2");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_AliasAttributed_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.WithOptionAliasAttributes);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<string>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(1);
            a.Description.Should().Be(null);
            a.Name.Should().Be("WithOptionAliasAttributes");
            a.Parent.Should().BeSameAs(o);
        }

        [Fact]
        public void Given_Type_Has_Colors_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.Colors);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("colors");
            o.Aliases.Should().Contain("Colors");
            o.Description.Should().Be("Colors");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_Colors_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.Colors);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<int>();
            a.Arity.MinimumNumberOfArguments.Should().Be(1);
            a.Arity.MaximumNumberOfArguments.Should().Be(int.MaxValue);
            a.Description.Should().Be(null);
            a.Name.Should().Be("Colors");
            a.Parent.Should().BeSameAs(o);
        }

        [Fact]
        public void Given_Type_Has_Flowers_Property()
        {
            var o = _command.GetOption<MyOptions>(x => x.Flowers);
            o.Should().NotBeNull();
            o.Aliases.Should().HaveCount(2);
            o.Aliases.Should().Contain("flowers");
            o.Aliases.Should().Contain("Flowers");
            o.Description.Should().Be("Flowers");
            o.IsHidden.Should().BeFalse();
            o.Parent.Should().BeSameAs(_command);
        }

        [Fact]
        public void Given_Flowers_Property_Has_Argument()
        {
            var o = _command.GetOption<MyOptions>(x => x.Flowers);
            var a = o?.Argument;
            a.Should().NotBeNull();
            a.ArgumentType.Should().Be<string>();
            a.Arity.MinimumNumberOfArguments.Should().Be(0);
            a.Arity.MaximumNumberOfArguments.Should().Be(int.MaxValue);
            a.Description.Should().Be(null);
            a.Name.Should().Be("Flowers");
            a.Parent.Should().BeSameAs(o);
        }
    }
}
