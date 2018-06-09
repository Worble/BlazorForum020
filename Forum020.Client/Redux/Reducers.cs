using BlazorRedux;
using Forum020.Client.Shared;
using Forum020.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Forum020.Client.Redux
{
    public class Reducers
    {
        public static ForumState ForumReducer(ForumState state, IAction action)
        {
            return new ForumState()
            {
                Boards = BoardsReducer(state.Boards, action),
                CurrentBoard = CurrentBoardReducer(state.CurrentBoard, action),
                ThreadViewType = ThreadViewTypeReducer(state.ThreadViewType, action),
                Content = ContentReducer(state.Content, action)
            };
        }

        private static string ContentReducer(string content, IAction action)
        {
            switch (action)
            {
                case AddQuoteAction a:
                    return content += ">>" + a.PostId + "\n";
                case ClearTextAction _:
                    return string.Empty;
                case UpdateTextAction a:
                    return a.Text;
                default: return content;
            }
        }

        private static ThreadView ThreadViewTypeReducer(ThreadView threadViewType, IAction action)
        {
            switch (action)
            {
                case ChangeThreadViewTypeAction a:
                    return a.ThreadViewType;
                default: return threadViewType;
            }
        }

        public static IEnumerable<BoardDTO> BoardsReducer(IEnumerable<BoardDTO> boards, IAction action)
        {
            switch (action)
            {
                case GetBoardsAction a:
                    return a.Boards;
                default: return boards;
            }
        }

        private static BoardDTO CurrentBoardReducer(BoardDTO currentBoard, IAction action)
        {
            switch (action)
            {
                case ClearThreadsAction _:
                    return new BoardDTO();
                case GetThreadsAction a:
                    if (currentBoard?.CurrentThread != null) { a.Board.CurrentThread = currentBoard.CurrentThread; }
                    a.Board.Threads.OrderByDescending(e => e.BumpDate ?? e.DateCreated);
                    return a.Board;
                case ClearPostsAction _:
                    if (currentBoard != null) { currentBoard.CurrentThread = new PostDTO(); }
                    return currentBoard;
                case GetPostsAction a:
                    if (currentBoard?.Id == a.Board.Id) { a.Board.Threads = currentBoard.Threads; }
                    a.Board.CurrentThread.Posts.OrderBy(e => e.DateCreated);
                    return a.Board;
                default:
                    return currentBoard;
            }
        }
    }
}
