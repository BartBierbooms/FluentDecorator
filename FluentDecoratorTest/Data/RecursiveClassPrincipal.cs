using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDecoratorTest.Data
{
    public class RecursiveClassPrincipal
    {
        public virtual RecursiveClassDependant DependantOn { get; set; }
        public int Count {get; set;}
    }
}
