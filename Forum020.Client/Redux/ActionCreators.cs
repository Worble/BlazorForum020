using BlazorRedux;
using Forum020.Shared;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using System.Text;
using Microsoft.JSInterop;
using System.Collections.Generic;
using Forum020.Client.Shared;

namespace Forum020.Client.Redux
{
    public class ActionCreators
    {
        public static async Task GetBoards(Dispatcher<IAction> dispatch, HttpClient http)
        {
            var uri = new UriBuilder(RoutePaths.Api + "boards").Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Get);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var boards = Json.Deserialize<IEnumerable<BoardDTO>>(await response.Content.ReadAsStringAsync());

                    dispatch(new GetBoardsAction
                    {
                        Boards = boards
                    });
                    break;

                default:
                    dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                    break;
            }
        }

        public static async Task GetThreads(Dispatcher<IAction> dispatch, HttpClient http, string boardName, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearThreadsAction());
            }

            var uri = new UriBuilder(RoutePaths.Api + boardName).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Get);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var board = Json.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                    dispatch(new GetThreadsAction
                    {
                        Board = board
                    });
                    break;

                default:
                    dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                    break;
            }
        }

        public static async Task GetPosts(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int threadId, bool clear)
        {
            if (clear)
            {
                dispatch(new ClearPostsAction());
            }

            var uri = new UriBuilder(RoutePaths.Api + boardName + "/" + threadId).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Get);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var board = Json.Deserialize<BoardDTO>(await response.Content.ReadAsStringAsync());

                    dispatch(new GetPostsAction
                    {
                        Board = board
                    });
                    break;

                default:
                    dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                    break;
            }
        }

        public static async Task PostThread(Dispatcher<IAction> dispatch, HttpClient http, string boardName, CreatePostDTO thread)
        {
            var uri = new UriBuilder(RoutePaths.Api + boardName).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Post, true, thread);

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

                default:
                    dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                    break;
            }
        }

        public static async Task PostPost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int thread, CreatePostDTO post)
        {
            var uri = new UriBuilder(RoutePaths.Api + boardName + "/" + thread).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Post, true, post);

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

                default:
                    dispatch(new SetErrorMessage() { Message = "Whoops! Something went wrong. Please try again later." });
                    break;
            }
        }

        public static async Task DeletePost(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            var uri = new UriBuilder(RoutePaths.Api + boardName + "/delete/" + postId).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Delete, true);

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

        public static async Task DeleteImage(Dispatcher<IAction> dispatch, HttpClient http, string boardName, int postId)
        {
            var uri = new UriBuilder(RoutePaths.Api + boardName + "/delete-image/" + postId).Uri;
            var response = await HttpHelper.PerformHttpRequest(uri, http, dispatch, HttpMethod.Delete, true);

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
    }
}
