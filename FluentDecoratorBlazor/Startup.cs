using Microsoft.AspNetCore.Components.Builder;
namespace FluentDecoratorBlazor
{
    public class Startup
    {
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
