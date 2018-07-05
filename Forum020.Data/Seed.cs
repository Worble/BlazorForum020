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
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post);
                    context.SaveChanges();

                    Post post4 = new Post()
                    {
                        Content = "Response to thread 1",
                        Thread = post,
                        Board = board,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post4);
                    context.SaveChanges();

                    var post2 = new Post()
                    {
                        Board = board,
                        Content = "Thread 2",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post2);
                    context.SaveChanges();

                    //board 2
                    var board2 = new Board()
                    {
                        Name = "Test",
                        NameShort = "tst",
                        Config = new Config()
                        {

                        }
                    };
                    context.Add(board2);
                    context.SaveChanges();

                    var post3 = new Post()
                    {
                        Board = board2,
                        Content = "Thread 3",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post3);
                    context.SaveChanges();


                    var post5 = new Post()
                    {
                        Board = board2,
                        Content = "Thread 4",
                        IsOp = true,
                        UserIdentifier = "TestUser"
                    };
                    context.Add(post5);
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
