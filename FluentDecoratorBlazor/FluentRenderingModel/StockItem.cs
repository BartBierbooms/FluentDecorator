using System;

namespace FluentDecoratorBlazor.FluentRenderingModel
{
    public class StockItem
    {
        public StockItem() {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; }
        public string Description { get; set; }
        public double Weight { get; set; }
    }
}
