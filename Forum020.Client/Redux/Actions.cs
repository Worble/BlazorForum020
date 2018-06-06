using BlazorRedux;
using Forum020.Client.Shared;
using Forum020.Shared;
using System.Collections.Generic;

namespace Forum020.Client.Redux
{
    public class GetBoardsAction : IAction
    {
        public IEnumerable<BoardDTO> Boards { get; set; }
    }

    public class GetThreadsAction : IAction
    {
        public BoardDTO Board { get; set; }
    }

    public class ClearThreadsAction : IAction { }

    public class GetPostsAction : IAction
    {
        public BoardDTO Board { get; set; }
    }

    public class ClearPostsAction : IAction { }

    public class ChangeThreadViewTypeAction : IAction
    {
        public ThreadView ThreadViewType { get; set; }
    }

    public class AddQuoteAction : IAction
    {
        public string PostId { get; set; }
    }

    public class ClearTextAction : IAction { }
}
