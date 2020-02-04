using System;

namespace FluentDecorator
{
    /// <summary>
    /// Attribute to only decoration only property with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LimitDecorationToAttribute : Attribute
    {
        public LimitDecorationToAttribute()
        {
        }
    }

}
