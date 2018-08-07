using Forum020.Data;
using Forum020.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum020.Data
{
    public static class Seed
    {
        public static void SeedDb(ForumContext context)
        {
            using (context.Database.BeginTransaction())
            {
                try
                {
                    //board 1
                    Board board = new Board()
                    {
                        Name = "Random",
                        NameShort = "b",
                        Config = new Config()
                        {
                            
                        }
                    };
                    context.Add(board);
                    context.SaveChanges();

                    Post post = new Post()
                    {
                        Board = board,
                        Content = "Thread 1",
                        IsOp = true,
                        UserIdentifier = "TestUser",
                        Posts = new List<Post>()
                        {
                            new Post()
                            {
                                Content = "Response to thread 1",
                                Board = board,
                                UserIdentifier = "TestUser"
                            }
                        }
                    };
                    context.Add(post);
                    context.SaveChanges();

                    //post = new Post()
                    //{
                    //    Content = "Response to thread 1",
                    //    Thread = post,
                    //    Board = board,
                    //    UserIdentifier = "TestUser"
                    //};
                    //context.Add(post);
                    //context.SaveChanges();

                    post = new Post()
                    {
                        Board = board,
                        Content = "Thread 2",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post);
                    context.SaveChanges();

                    //board 2
                    board = new Board()
                    {
                        Name = "Test",
                        NameShort = "tst",
                        Config = new Config()
                        {

                        }
                    };
                    context.Add(board);
                    context.SaveChanges();

                    post = new Post()
                    {
                        Board = board,
                        Content = "Thread 3",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post);
                    context.SaveChanges();


                    post = new Post()
                    {
                        Board = board,
                        Content = "Thread 4",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post);
                    context.SaveChanges();

                    var reportType = new ReportType() { Name = "Illegal Content" };
                    context.Add(reportType);
                    context.SaveChanges();

                    reportType = new ReportType() { Name = "Rule Violation" };
                    context.Add(reportType);
                    context.SaveChanges();

                    context.Database.CommitTransaction();
                }
                catch
                {
                    context.Database.RollbackTransaction();
                    throw;
                }
            }
        }
    }
}
