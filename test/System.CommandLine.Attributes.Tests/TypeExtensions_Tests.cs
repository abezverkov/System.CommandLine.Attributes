using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace System.CommandLine.Attributes.Tests
{
    public class When_Using_TypeExtensions
    {
        public class TestClass
        {
            public int MyProperty { get; set; }
            public string MyOtherProperty { get; set; }
        }

        [Fact]
        public void Given_A_List_String()
        {
            var l = new List<string>();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<string>();
        }

        [Fact]
        public void Given_A_List_Int()
        {
            var l = new List<int>();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<int>();
        }

        [Fact]
        public void Given_A_List_TestClass()
        {
            var l = new List<TestClass>();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<TestClass>();
        }

        [Fact]
        public void Given_A_Collection_Int()
        {
            var l = new Collection<int>();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<int>();
        }

        [Fact]
        public void Given_An_Array_String()
        {
            var l = new string[0];
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<string>();
        }

        [Fact]
        public void Given_An_Array()
        {
            var l = new ArrayList();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeFalse();
            t.GetEnumerableType().Should().Be<object>();
        }

        [Fact]
        public void Given_An_Int()
        {
            var l = 1;
            var t = l.GetType();

            t.IsEnumerable().Should().BeFalse();
            t.IsEnumerableT().Should().BeFalse();
            t.GetEnumerableType().Should().BeNull();
        }
        [Fact]
        public void Given_A_String()
        {
            var l = "somestring";
            var t = l.GetType();

            t.IsEnumerable().Should().BeFalse();
            t.IsEnumerableT().Should().BeFalse();
            t.GetEnumerableType().Should().BeNull();
        }

        [Fact]
        public void Given_A_String_CastAs_CharArray()
        {
            var l = "somestring".ToCharArray();
            var t = l.GetType();

            t.IsEnumerable().Should().BeTrue();
            t.IsEnumerableT().Should().BeTrue();
            t.GetEnumerableType().Should().Be<char>();
        }



    }
}
