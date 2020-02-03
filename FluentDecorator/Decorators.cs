namespace FluentDecorator
{
    /// <summary>
    /// Concrete implementation of a decorators class. You can use your own decorator class as long as it inherits from DecoratorsAbstract.
    /// </summary>
    public class Decorators : DecoratorsAbstract
    {
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
    }
}
