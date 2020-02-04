using System;
using FluentDecorator;

namespace FluentDecoratorTest.Data
{
    public class ClassWithMember
    {
        [ExcludeFromDecoration]
        public SimpleClass SimpleClass { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
