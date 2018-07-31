using BlazorRedux;
using Forum020.Client.Redux;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Forum020.Client.Shared
{
    public static class HttpHelper
    {
        public async static Task<HttpResponseMessage> PerformHttpRequest(Uri uri, HttpClient http, Dispatcher<IAction> dispatch, HttpMethod method, bool requiresCredentials = false, object content = null)
        {
            dispatch(new SetIsLoading() { IsLoading = true });
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = uri
                };

                if (content != null)
                {
                    requestMessage.Content = new StringContent(Json.Serialize(content), Encoding.UTF8,
                        "application/json");
                }

                if (requiresCredentials)
                {
                    BrowserHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;
                    requestMessage.Properties.Add("BrowserHttpMessageHandler.FetchArgs", new { mode = "cors" });
                }

                return await http.SendAsync(requestMessage);
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
    }
}
