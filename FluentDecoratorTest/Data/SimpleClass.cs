using System;
using FluentDecorator;
namespace FluentDecoratorTest.Data
{
    public class SimpleClass
    {
        [ExcludeFromDecoration]
        public string Name { get; set; }

        [LimitDecorationTo]
        public int Age { get; set; }
        public DateTime UpdateTime { get; set; }
        public Single? Aantal_uren { get; set; }
        public int Aantal_urenTranslated
        {
            get => (int)Aantal_uren.GetValueOrDefault(0);
            set => Aantal_uren = value;
        }
    }
}
