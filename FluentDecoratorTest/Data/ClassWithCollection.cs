using System;
using System.Collections.Generic;

namespace FluentDecoratorTest.Data
{
    public class ClassWithCollection
    {
        public Guid Id { get; set; }
        public List<SimpleClass> SimpleClasses { get; set; }  
}
}
