using FluentDecorator;
using System;

namespace FluentDecoratorTest.Data
{
    public class ClassWithDynamicProperty
    {
        public dynamic ADynamicPrioperty { get; set; }
        public Guid Id { get; set; }

        [ExcludeFromDecoration]
        public SimpleClass SimpleClass { get; set; }

    }
}
