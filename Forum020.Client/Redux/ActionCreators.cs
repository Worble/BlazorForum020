using BlazorRedux;
using Forum020.Shared;
using System.Net.Http;
using Microsoft.AspNetCore.Blazor;
using System.Threading.Tasks;
using System;
using System.Runtime.Serialization;

namespace Forum020.Client.Redux
{
    public class ActionCreators
    {
        public static async Task GetBoards(Dispatcher<IAction> dispatch, HttpClient http)
        {
            var boards = await http.GetJsonAsync<BoardDTO[]>(RoutePaths.Api + "boards");
            dispatch(new GetBoardsAction
            {
                Boards = boards
            });
        }

        public static async Task GetThreads(Dispatcher<IAction> dispatch, HttpClient http, string boardName, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearThreadsAction());
            }

            var board = await http.GetJsonAsync<BoardDTO>(RoutePaths.Api + boardName);

            dispatch(new GetThreadsAction
            {
                Board = board
            });
        }

        public static async Task GetPosts(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int threadId, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearPostsAction());
            }

            var board = await http.GetJsonAsync<BoardDTO>(RoutePaths.Api + boardName + "/" + threadId);

            dispatch(new GetPostsAction
            {
                Board = board
            });
        }

        public static async Task PostThread(Dispatcher<IAction> dispatch, HttpClient http, string boardName, PostDTO thread)
        {
            var board = await http.PostJsonAsync<BoardDTO>(RoutePaths.Api + boardName, thread);

            dispatch(new GetPostsAction
            {
                Board = board
            });
            
        }

        public static async Task PostPost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int thread, PostDTO post)
        {
            try
            {
                var board = await http.PostJsonAsync<BoardDTO>(RoutePaths.Api + boardName + "/" + thread, post);
                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch (SerializationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
