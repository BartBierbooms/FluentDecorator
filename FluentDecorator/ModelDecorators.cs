using System;
using System.Collections;
using System.Collections.Generic;


namespace FluentDecorator
{
    /// <summary>
    /// Class tthat implements the IModelDecorators interface
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <typeparam name="TS">The decorator type</typeparam>
    public class ModelDecorators<T, TS> : IModelDecorators<T, TS>
        where T : new()
        where TS : DecoratorsAbstract, new()
    {
        /// <summary>
        /// The model of type T
        /// </summary>
        public T Val { get; }

        /// <summary>
        /// Dictionary with propertypath as a key and the decorator instance as its value 
        /// </summary>
        public Dictionary<string, TS> PropertyDecorators { get; } = new Dictionary<string, TS>();

        /// <summary>
        /// The decorators. TS must inherit from DecoratorsAbstract
        /// </summary>
        public TS Decorators { get; }
        public ModelDecorators() { }
        public ModelDecorators(T val, TS decorators)
        {

            Val = val;
            Decorators = decorators;
            PropertyDecorators = GetPropertyNames(typeof(T), PropertyDecorators);
        }

        private Dictionary<string, TS> GetPropertyNames(Type type, Dictionary<string, TS> propertyList, string memberPath = "")
        {
            var prpInfo = type.GetProperties();

            for (int i = 0; i < prpInfo.Length; i++)
            {
                if (!prpInfo[i].PropertyType.IsValueType
                    && prpInfo[i].PropertyType != typeof(string))
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(prpInfo[i].PropertyType) || prpInfo[i].PropertyType == typeof(string))
                    {
                        propertyList.Add($"{memberPath}{prpInfo[i].Name}", new TS());
                        var descendingPropertyList = GetPropertyNames(prpInfo[i].PropertyType, new Dictionary<string, TS>(), $"{memberPath}{prpInfo[i].Name}.");
                        foreach (var prop in descendingPropertyList)
                        {
                            propertyList.Add(prop.Key, prop.Value);
                        }
                    }
                }
                else
                {
                    propertyList.Add($"{memberPath}{prpInfo[i].Name}", new TS());
                }
            }
            return propertyList;
        }

    }
}
