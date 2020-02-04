using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDecoratorTest.Data
{
    public class RecursiveClassDependant
    {
        public virtual RecursiveClassPrincipal PrincipalFrom { get; set; }
        public int Number { get; set; }

    }
}
