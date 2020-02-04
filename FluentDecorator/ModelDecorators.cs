using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
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
        public ModelDecorators(T val, TS decorators, FilterSetting filter = null)
        {

            Val = val;
            Decorators = decorators;
            PropertyDecorators = GetPropertyNames(typeof(T), PropertyDecorators, filter, "", null);
        }

        private Dictionary<string, TS> GetPropertyNames(Type type, Dictionary<string, TS> propertyList, FilterSetting filter, string memberPath, Type callingType )
        {
            
            var reflectedPrpInfos = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (filter != null) 
            {
                if (filter.Include)
                {
                    reflectedPrpInfos = reflectedPrpInfos.Where(prp => prp.GetCustomAttributes<LimitDecorationToAttribute>().Any()).ToArray();
                }
                else 
                {
                    reflectedPrpInfos = reflectedPrpInfos.Where(prp => !prp.GetCustomAttributes<ExcludeFromDecorationAttribute>().Any()).ToArray();
                }
            }

            IList<PropertyInfo> prpInfos;
            if (callingType != null)
            {
                prpInfos = reflectedPrpInfos.Where(pi => !pi.PropertyType.Equals(callingType)).ToList();
            }
            else 
            {
                prpInfos = reflectedPrpInfos;
            }
            for (int i = 0; i < prpInfos.Count; i++)
            {
                if (!prpInfos[i].PropertyType.IsValueType
                    && prpInfos[i].PropertyType != typeof(string))
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(prpInfos[i].PropertyType) || prpInfos[i].PropertyType == typeof(string))
                    {
                        propertyList.Add($"{memberPath}{prpInfos[i].Name}", new TS());
                        var descendingPropertyList = GetPropertyNames(prpInfos[i].PropertyType, new Dictionary<string, TS>(),filter, $"{memberPath}{prpInfos[i].Name}.", type);
                        foreach (var prop in descendingPropertyList)
                        {
                            propertyList.Add(prop.Key, prop.Value);
                        }
                    }
                }
                else
                {
                    propertyList.Add($"{memberPath}{prpInfos[i].Name}", new TS());
                }
            }
            return propertyList;
        }

    }
}
