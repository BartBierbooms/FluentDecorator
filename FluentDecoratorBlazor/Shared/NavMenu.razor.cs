﻿namespace FluentDecoratorBlazor.Shared
{
    partial class NavMenu 
    {

        protected bool collapseNavMenu = true;
        protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
