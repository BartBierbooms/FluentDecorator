namespace FluentDecorator
{
    /// <summary>
    /// Class that determines if the properties to decorate are filtered base (exclude or include) on the FilterDecorate property attribute. 
    /// Use Filter attributes in complex recursive property chanining scenario's.
    /// </summary>
    public class FilterSetting 
    {
        public FilterSetting(bool include)
        {
            Include = include;
        }
        public bool Include { get; }
    }
}
