using BlazorRedux;
using Forum020.Client.Redux;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Forum020.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddReduxStore<ForumState, IAction>(new ForumState(), Reducers.ForumReducer);
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
