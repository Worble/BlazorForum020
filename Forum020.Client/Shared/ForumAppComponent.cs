using BlazorRedux;
using Forum020.Client.Redux;
using System;

namespace Forum020.Client.Shared
{
    public class ForumAppComponent : ReduxComponent<ForumState, IAction>
    {

    }

    public enum View
    {
        Thread,
        Post
    }
}
