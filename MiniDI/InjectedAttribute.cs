using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniDI
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class InjectedAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        //private readonly string positionalString;

        // This is a positional argument
        public InjectedAttribute()
        {
        }

        //public string PositionalString { get; private set; }

        //// This is a named argument
        //public int NamedInt { get; set; }
    }
}
