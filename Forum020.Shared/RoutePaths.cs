﻿namespace Forum020.Shared
{
    public static class RoutePaths
    {
        public static string Boards() => "/";
        public static string Threads(string Board) => "/" + Board;
        public static string Posts(string Board, int Thread) => "/" + Board + "/" + Thread;
    }
}