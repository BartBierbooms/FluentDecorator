using System;
using System.Linq.Expressions;

namespace FluentDecorator
{
    public static class ModelDecorators
    {

        public delegate IDecorator WithDecorators<in TI, out IDecorator>(TI i);

        /// <summary>
        /// Initialize method to start a Decorator Pipeline Fluent Definition
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> Init<TI, TS>()
            where TI : new()
            where TS : DecoratorsAbstract, new()
        {
            return input =>
            {
                return new ModelDecorators<TI, TS>(input, new TS());
            };
        }

        /// <summary>
        /// Initialize method to start A Decorator Pipeline Fluent Definition
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="decorators">Func delegate to create a decorator type</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> Init<TI, TS>(Func<TS> decorators)
            where TI : new()
            where TS : DecoratorsAbstract, new()
        {
            return input =>
            {
                return new ModelDecorators<TI, TS>(input, decorators());
            };
        }

        /// <summary>
        /// Get the decorator instance for the property returned by the expression
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="expr">Expression returning the property that is decorated with a decorator class</param>
        /// <returns>Decorator</returns>
        public static TS GetFor<TI, TProperty, TS>(this IModelDecorators<TI, TS> source, Expression<Func<TI, TProperty>> expr)
            where TI : new()
            where TS : DecoratorsAbstract
        {
            var prpName = PropertyExpressionExtension.GetMemberName(expr.Body);
            if (source.PropertyDecorators.TryGetValue(prpName, out var decorators))
            {
                return decorators;
            }
            throw new InvalidOperationException($"No decorators attached to {prpName}. Is this an IEnumerable?");
        }

        /// <summary>
        /// Define a decoration action, given a predicate, for the property returned by the expression
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="predicate">Predicate determinig which apply action to call</param>
        /// <param name="expr">Expression returning the property that is decorated with a decorator class</param>
        /// <param name="applyOnTrue">Action excecuted when predicate returns true. Action for setting properties of the decorator class</param>
        /// <param name="applyOnFalse">Action excecuted when predicate returns false. Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModelPropIfThenElse<TI, TProperty, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Func<TI, bool> predicate,
            Expression<Func<TI, TProperty>> expr,
            Action<TS> applyOnTrue,
            Action<TS> applyOnFalse)
            where TI : new()
            where TS : DecoratorsAbstract, new()
        {
            return input =>
            {
                var prpName = PropertyExpressionExtension.GetMemberName(expr.Body);
                var ret = source(input);
                if (ret.Val != null)
                {
                    if (ret.PropertyDecorators.TryGetValue(prpName, out var decorators))
                    {
                        if (predicate(ret.Val))
                        {
                            applyOnTrue(decorators);
                        }
                        else
                        {
                            applyOnFalse(decorators);
                        }
                    }
                }
                return ret;
            };
        }

        /// <summary>
        /// Define the decoration action, when the predicate is met, for the property returned by the expression
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="predicate">Predicate determinig if the apply action is called</param>
        /// <param name="expr">Expression returning the property that is decorated with a decorator class</param>
        /// <param name="apply">Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModelPropWhen<TI, TProperty, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Func<TI, bool> predicate,
            Expression<Func<TI, TProperty>> expr,
            Action<TS> apply)
            where TI : new()
            where TS : DecoratorsAbstract, new()
        {
            return OnModelPropIfThenElse(source, predicate, expr, apply, ts => { });
        }

        /// <summary>
        /// Define the decoration action for the property returned by the expression
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="expr">Expression returning the property that is decorated with a decorator class</param>
        /// <param name="apply">Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModelProp<TI, TProperty, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Expression<Func<TI, TProperty>> expr,
            Action<TS> apply)
            where TI : new()
            where TS : DecoratorsAbstract, new()
        {
            return source.OnModelPropWhen(x => true, expr, apply);
        }

        /// <summary>
        /// Define the decoration action, when the predicate is met, for the model
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="predicate">Predicate determinig if the apply action is called</param>
        /// <param name="apply">Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModelWhen<TI, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Func<TI, bool> predicate,
            Action<TS> apply)
            where TI : new()
            where TS : DecoratorsAbstract
        {
            return OnModelIfThenElse(source, predicate, apply, ts => { });
        }

        /// <summary>
        /// Define a decoration action, given a predicate, for the model
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="predicate">Predicate determinig which apply action to call</param>
        /// <param name="applyOnTrue">Action excecuted when predicate returns true. Action for setting properties of the decorator class</param>
        /// <param name="applyOnFalse">Action excecuted when predicate returns false. Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModelIfThenElse<TI, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Func<TI, bool> predicate,
            Action<TS> applyOnTrue,
            Action<TS> applyOnFalse)
            where TI : new()
            where TS : DecoratorsAbstract
        {
            return input =>
            {
                var ret = source(input);
                if (ret.Val != null)
                {
                    if (predicate(ret.Val))
                    {
                        applyOnTrue(ret.Decorators);
                    }
                    else
                    {
                        applyOnFalse(ret.Decorators);
                    }
                }
                return ret;
            };
        }

        /// <summary>
        /// Define a decoration action for the model
        /// </summary>
        /// <typeparam name="TI">The model type</typeparam>
        /// <typeparam name="TS">The decorator type</typeparam>
        /// <param name="source">The WithDecorators delegate. Use the method Init method to create this delegate or use a returned WithDecorators delegate of one of the Extension methods</param>
        /// <param name="apply">Action for setting properties of the decorator class</param>
        /// <returns>WithDecorators delegate</returns>
        public static WithDecorators<TI, IModelDecorators<TI, TS>> OnModel<TI, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            Action<TS> apply)
            where TI : new()
            where TS : DecoratorsAbstract
        {
            return source.OnModelWhen(x => true, apply);
        }
    }
}
