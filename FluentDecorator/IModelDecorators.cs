using System.Collections.Generic;

namespace FluentDecorator
{
    /// <summary>
    /// Interface combinig the model, decorator on the model and the decorators on the properties of the model.
    /// Properties of type IEnumerable are excluded.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <typeparam name="TS">The decorator type</typeparam>
    public interface IModelDecorators<T, TS>
        where T : new()
        where TS : DecoratorsAbstract
    {
        /// <summary>
        /// The model of type T
        /// </summary>
        T Val { get; }

        /// <summary>
        /// The decorator of type TS. TS must inherit from DecoratorsAbstract
        /// </summary>
        TS Decorators { get; }

        /// <summary>
        /// Dictionary with propertypath as a key and the decorator instance as its value 
        /// </summary>
        Dictionary<string, TS> PropertyDecorators { get; }
    }
}
