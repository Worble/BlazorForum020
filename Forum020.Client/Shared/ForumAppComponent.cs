using BlazorRedux;
using Forum020.Client.Redux;

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

    public enum ThreadView
    {
        Standard,
        Catalogue
    }



}
