using System;

namespace FluentDecorator
{
    /// <summary>
    /// Attribute to exclude a property for decoration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeFromDecorationAttribute : Attribute
    {
        public ExcludeFromDecorationAttribute() 
        {
        }
    }

}
