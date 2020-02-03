# FluentDecorator (created when doing Blazor). Why?

I was creating a Blazor project with some rendering logic. I was setting readonly, disabled, class etc. attributes to variables and methods. It was spread across the markup file. I was wondering whether this kind of rendering logic couldn't be defined more centralized and made descriptive, given you more inside what the rendering logic actually is about and what it involves.

Secondly with Blazor the distinction between server-side and client-side objects disappears. You can use one class for both. But both sides keep their own context. Readonly server-side is used for properties ressembling database keys, while at the client side it targets rendering purposes.

## The solution
The solution to both was: use a shared model for client and server-side, but decorate the model on the client side with rendering properties. Secondly, define these decorators by using a fluent interface. 
In the end it looks like (c# class behind code):
```
            modelDecorationDefinition = ModelDecorators.Init<Ship, ExtendedDecorators>()
                .OnModelIfThenElse(WhenThereIsStock(), EnableDisembark(), DisableDisembark())
                .OnModelPropIfThenElse(s => s.NextInRowToApproveDisembark() is Captain,
                    s => s.Captain,
                    prp => prp.Class = classShow,
                    prp => prp.Class = classHide)
                .OnModelPropIfThenElse(s => s.Status == DisembarkStatus.WaitOnSteerman,
                    s => s.Steersman,
                    prp => prp.Class = classShow,
                    prp => prp.Class = classHide)
                .OnModelPropWhen(s =>
                    s.Status >= DisembarkStatus.DisembarkCanStart
                    && s.Status < DisembarkStatus.DisembarkFinished
                    , s => s.Status
                    , dec => dec.Visible = true);

            modelDecorated = modelDecorationDefinition(ship);
```
and the markup code could be:
```
    <div class="@ModelDecorated.GetFor(s => s.Captain).Class">
        @*etc. etc.*@
    </div>
    @if (modelDecorated.GetFor(x => x.Status).Visible)
    {
        @*etc. etc.*@
    }
```
## Solution explained
The model (in this case ship) isn't changed, it doesn't have rendering properties like readonly and enabled. Model logic still resides in the model class, for example: `NextInRowToApproveDisembark` and `EvaluateStatus`.

The rendering logic is centralized in `modelDecorationDefinition`. The model is decorated (extended if you want) with properties from the ExtendedDecorators class (`Enabled`, `Visible` and `Class`). This is defined by the generic type parameters of the `ModelDecorators.Init<Ship, ExtendedDecorators>` method. Instead of the `ExtendedDecorators`, you can use any other class as long as it inherits from the `DecoratorsAbstract` class, which is a marker class (= no properties in it). In other words you are completly free in defining your own decoration properties.

The decorations are done on the class itself and the properties of the class. If a property is a reference type, like `Captain`, the decoration descends also these classes ending up with an `ExtendedDecorators` instance for the model, every property of the model and every property of the property chain like `Captain` etc.. IEnumerables are excluded from this chaining mechanism.

### Definition versus Execution
The fluent interface which starts with `ModelDecorators.Init<TI, TS>`, is a func delegate. It is nothing else but piped function pointers, which are only executed when invoked with a TI instance (in our case a ship). 

You can extend the fluent interface with your own functions. Say you want to combine two decorator you just define 
```
        public static WithDecorators<TI, IModelDecorators<TI, TS>> Combine<TI, TJ, TS>(
            this WithDecorators<TI, IModelDecorators<TI, TS>> source,
            WithDecorators<TJ, IModelDecorators<TJ, TS>> combineWith,
            TJ combinedValue)
            where TI : new()
            where TJ : new()
            where TS : DecoratorsAbstract, new()
        {
            return input =>
            {
                var ret = source(input);
                var retCombineWith = combineWith(combinedValue);
                //do your combining stuff here
                return ret;
            };
        }


```
When do we want to execute this pipeline? In Blazor, you probably want it after a model change, but just before the rendering starts. The `ShouldRender override` seems the perfect place. If the state changes and is rendered, your model decorations are updated as well.
```
        protected override bool ShouldRender()
        {
            modelDecorated = modelDecorationDefinition(ship);
            return base.ShouldRender();
        }
```
# Example
I included an example which also deals with IEnumerables (in which you add decorations item based). 