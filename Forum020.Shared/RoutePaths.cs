namespace Forum020.Shared
{
    public static class RoutePaths
    {
        public static string Api => "http://localhost:8610/api/";
        public static string BoardsRoute() => "/";
        public static string ThreadsRoute(string board) => "/" + board;
        public static string PostsRoute(string board, int threadId) => "/" + board + "/" + threadId;
        public static string SinglePost(string board, int postId) => board + "/" + postId;
    }
}
