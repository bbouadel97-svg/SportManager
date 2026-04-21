using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SportManager.Maui
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            InitializeComponent();
            MainPage = services.GetRequiredService<MainPage>();
        }
    }
}

