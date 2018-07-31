using BlazorRedux;
using Forum020.Shared;
using System.Net.Http;
using Microsoft.AspNetCore.Blazor;
using System.Threading.Tasks;
using System;
using System.Net;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using System.Text;
using Microsoft.JSInterop;

namespace Forum020.Client.Redux
{
    public class ActionCreators
    {
        public static async Task GetBoards(Dispatcher<IAction> dispatch, HttpClient http)
        {
            dispatch(new SetIsLoading() { IsLoading = true });
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
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task GetThreads(Dispatcher<IAction> dispatch, HttpClient http, string boardName, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearThreadsAction());
            }

            dispatch(new SetIsLoading() { IsLoading = true });

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
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task GetPosts(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int threadId, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearPostsAction());
            }

            dispatch(new SetIsLoading() { IsLoading = true });

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
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task PostThread(Dispatcher<IAction> dispatch, HttpClient http, string boardName, CreatePostDTO thread)
        {
            dispatch(new SetIsLoading() { IsLoading = true });

            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName).Uri,
                    Content = new StringContent(Json.Serialize(thread), Encoding.UTF8,
                                    "application/json")
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });
                
                var result = await http.SendAsync(requestMessage);

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var board = Json.Deserialize<BoardDTO>(await result.Content.ReadAsStringAsync());

                        dispatch(new GetPostsAction
                        {
                            Board = board
                        });
                        break;

                    case HttpStatusCode.BadRequest:
                        dispatch(new SetErrorMessage() { Message = await result.Content.ReadAsStringAsync() });
                        break;

                    default:
                        dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                        break;
                }                
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task PostPost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int thread, CreatePostDTO post)
        {
            dispatch(new SetIsLoading() { IsLoading = true });

            try
            {
                BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new UriBuilder(RoutePaths.Api + boardName + "/" + thread).Uri,
                    Content = new StringContent(Json.Serialize(post), Encoding.UTF8,
                                    "application/json")
                };
                requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });

                var result = await http.SendAsync(requestMessage);

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var board = Json.Deserialize<BoardDTO>(await result.Content.ReadAsStringAsync());

                        dispatch(new GetPostsAction
                        {
                            Board = board
                        });
                        break;

                    case HttpStatusCode.BadRequest:
                        dispatch(new SetErrorMessage() { Message = await result.Content.ReadAsStringAsync() });
                        break;

                    default:
                        dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                        break;
                }                
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task DeletePost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            dispatch(new SetIsLoading() { IsLoading = true });

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

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var board = Json.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                        dispatch(new GetPostsAction
                        {
                            Board = board
                        });
                        break;

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        dispatch(new SetErrorMessage() { Message = "You are not the owner of this post." });
                        break;

                    default:
                        dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                        break;
                }
            }
            catch(Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }

        public static async Task DeleteImage(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            dispatch(new SetIsLoading() { IsLoading = true });

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

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var board = Json.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                        dispatch(new GetPostsAction
                        {
                            Board = board
                        });
                        break;

                    case HttpStatusCode.BadRequest:
                        dispatch(new SetErrorMessage() { Message = await response.Content.ReadAsStringAsync() });
                        break;

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        dispatch(new SetErrorMessage() { Message = "You are not the owner of this post." });
                        break;

                    default:
                        dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                        break;
                }                
            }
            catch (Exception e)
            {
                dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                Console.WriteLine(e);
            }
            finally
            {
                dispatch(new SetIsLoading() { IsLoading = false });
            }
        }
    }
}
