using BlazorRedux;
using Forum020.Shared;
using System.Net.Http;
using Microsoft.AspNetCore.Blazor;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using System.Text;

namespace Forum020.Client.Redux
{
    public class ActionCreators
    {
        public static async Task GetBoards(Dispatcher<IAction> dispatch, HttpClient http)
        {
            try
            {
                var boards = await http.GetJsonAsync<BoardDTO[]>(RoutePaths.Api + "boards");
                dispatch(new GetBoardsAction
                {
                    Boards = boards
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Try again later." });
                Console.WriteLine(e);
            }
        }

        public static async Task GetThreads(Dispatcher<IAction> dispatch, HttpClient http, string boardName, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearThreadsAction());
            }

            try
            {
                var board = await http.GetJsonAsync<BoardDTO>(RoutePaths.Api + boardName);

                dispatch(new GetThreadsAction
                {
                    Board = board
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Try again later." });
                Console.WriteLine(e);
            }
        }

        public static async Task GetPosts(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int threadId, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearPostsAction());
            }

            try
            {
                var board = await http.GetJsonAsync<BoardDTO>(RoutePaths.Api + boardName + "/" + threadId);

                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Try again later." });
                Console.WriteLine(e);
            }
        }

        public static async Task PostThread(Dispatcher<IAction> dispatch, HttpClient http, string boardName, PostDTO thread)
        {
            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName).Uri,
                    Content = new StringContent(JsonUtil.Serialize(thread), Encoding.UTF8,
                                    "application/json")
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });
                
                var result = await http.SendAsync(requestMessage);

                var board = JsonUtil.Deserialize<BoardDTO>(await result.Content.ReadAsStringAsync());

                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Reformat your post or try again later." });
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task PostPost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int thread, PostDTO post)
        {
            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName + "/" + thread).Uri,
                    Content = new StringContent(JsonUtil.Serialize(post), Encoding.UTF8,
                                    "application/json")
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });

                var result = await http.SendAsync(requestMessage);
                var board = JsonUtil.Deserialize<BoardDTO>(await result.Content.ReadAsStringAsync());

                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Reformat your post or try again later." });
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task DeletePost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName + "/delete/" + postId).Uri
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });

                var response = await http.SendAsync(requestMessage);
                var board = JsonUtil.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch(Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "You are either not the owner of this post, or the server is down" });
                Console.WriteLine(e);
            }
        }

        public static async Task DeleteImage(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName + "/delete-image/" + postId).Uri
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });

                var response = await http.SendAsync(requestMessage);
                var board = JsonUtil.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                dispatch(new GetPostsAction
                {
                    Board = board
                });
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "You are either not the owner of this post, or the server is down" });
                Console.WriteLine(e);
            }
        }
    }
}
