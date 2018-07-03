using BlazorRedux;
using Forum020.Client.Redux;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace Forum020.Client.Shared
{
    public class ForumAppComponent : ReduxComponent<ForumState, IAction>
    {
    }

    public class ForumAppLayout : ForumAppComponent
    {
        [Parameter]
        protected RenderFragment Body { get; set; }
    }

    public enum View
    {
        Thread,
        Post
    }

    public enum ThreadView
    {
        Standard,
        Catalogue
    }
}
