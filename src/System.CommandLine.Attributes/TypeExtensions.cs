using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type t, Type ti)
        {
            if (ti == null)
            {
                throw new ArgumentNullException(nameof(ti));
            }
            if (!ti.IsInterface)
            {
                throw new ArgumentException("ti must be an interface",nameof(ti));
            }

            return  t.GetInterface(ti.Name) != null;
        }

        public static bool IsEnumerable(this Type t)
        {
            if (t == typeof(string)) return false;
            return t.ImplementsInterface(typeof(IEnumerable));
        }

        public static bool IsEnumerableT(this Type t)
        {
            if (t == typeof(string)) return false;
            return t.ImplementsInterface(typeof(IEnumerable<>));
        }

        public static Type GetEnumerableType(this Type t)
        {
            if (t.IsEnumerableT())
            {
                return t.GetInterface(typeof(IEnumerable<>).Name)
                    .GenericTypeArguments[0];
            }
            else if (t.IsEnumerable())
            {
                return typeof(object);
            }
            return null;
        }
    }
}
