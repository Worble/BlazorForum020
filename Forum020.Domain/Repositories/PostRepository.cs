using Forum020.Data;
using Forum020.Data.Entities;
using Forum020.Domain.Repositories.Interfaces;
using Forum020.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Forum020.Domain.Repositories
{
    public class PostRepository : IPostRepository
    {
        private ForumContext _context;

        public PostRepository(ForumContext context)
        {
            _context = context;
        }

        public async Task<BoardDTO> GetAllThreadsForBoard(string boardName)
        {
            return await _context.Boards.Select(e => new BoardDTO()
            {
                Id = e.Id,
                DateCreated = e.DateCreated,
                DateEdited = e.DateEdited,
                Name = e.Name,
                NameShort = e.NameShort,
                Threads = e.Threads
                    .Where(y => y.IsOp == true && !y.IsArchived)
                    .OrderByDescending(y => y.BumpDate ?? y.DateCreated)
                    .Select(y => new PostDTO()
                    {
                        Content = y.Content,
                        BumpDate = y.BumpDate,
                        DateCreated = y.DateCreated,
                        DateEdited = y.DateEdited,
                        Id = y.IdEffective,
                        IsOp = y.IsOp,
                        BoardId = y.BoardId,
                        ImageUrl = y.ImageUrl,
                        ThumbnailUrl = y.ThumbnailUrl,
                        IsArchived = y.IsArchived
                    })               
            })
            .FirstOrDefaultAsync(e => e.NameShort == boardName);
        }

        public async Task<BoardDTO> GetAllPostsForThread(string boardName, int threadId)
        {
            return await _context.Boards.Select(e => new BoardDTO()
            {
                Id = e.Id,
                DateCreated = e.DateCreated,
                DateEdited = e.DateEdited,
                Name = e.Name,
                NameShort = e.NameShort,
                CurrentThread = e.Threads
                    .Select(y => new PostDTO()
                    {
                        BoardId = y.BoardId,
                        Content = y.Content,
                        BumpDate = y.BumpDate,
                        DateCreated = y.DateCreated,
                        DateEdited = y.DateEdited,
                        Id = y.IdEffective,
                        IsOp = y.IsOp,
                        ImageUrl = y.ImageUrl,
                        ThumbnailUrl = y.ThumbnailUrl,
                        IsArchived = y.IsArchived,
                        Posts = y.Posts
                            .OrderBy(x => x.DateCreated)
                            .Select(x => new PostDTO()
                            {
                                Content = x.Content,
                                BumpDate = x.BumpDate,
                                DateCreated = x.DateCreated,
                                DateEdited = x.DateEdited,
                                Id = x.IdEffective,
                                IsOp = x.IsOp,
                                ThreadId = y.IdEffective,
                                BoardId = x.BoardId,
                                ImageUrl = x.ImageUrl,
                                ThumbnailUrl = x.ThumbnailUrl
                            })
                    })
                    .FirstOrDefault(y => y.Id == threadId)
            })
            .FirstOrDefaultAsync(e => e.NameShort == boardName);
        }

        public async Task<Post> PostThread(string boardName, PostDTO thread)
        {
            var board = await _context.Boards.FirstOrDefaultAsync(e => e.NameShort == boardName);
            var post = new Post()
            {
                Content = thread.Content,
                Board = board,
                IsOp = true,
                ImageChecksum = thread.ImageChecksum,
                ImageUrl = thread.ImageUrl,
                ThumbnailUrl = thread.ThumbnailUrl
            };

            var entity = _context.Posts.Add(post).Entity;

            await BumpThreads(boardName);

            return entity;
        }

        public async Task<Post> PostPost(string boardName, int threadId, PostDTO post)
        {
            var thread = await _context.Posts
                .Include(e => e.Board)
                    .ThenInclude(e => e.Config)
                .Include(e => e.Posts)
                .FirstOrDefaultAsync(e => e.Board.NameShort == boardName && e.IdEffective == threadId && !e.IsArchived);

            if(thread == null) return null;


            var postEntity = new Post()
            {
                Content = post.Content,
                Board = thread.Board,
                IsOp = false,
                Thread = thread,
                ImageChecksum = post.ImageChecksum,
                ImageUrl = post.ImageUrl,
                ThumbnailUrl = post.ThumbnailUrl
            };

            if (thread.Posts.Count() < thread.Board.Config.MaximumReplyCount)
            {
                thread.BumpDate = DateTime.UtcNow;
            }

            _context.Posts.Update(thread);
            return _context.Posts.Add(postEntity).Entity;
        }

        private async Task BumpThreads(string boardName)
        {
            var board = await _context.Boards.Select(e => new
            {
                e.Id,
                Threads = e.Threads
                    .Where(y => !y.IsArchived && y.IsOp)
                    .OrderBy(y => y.BumpDate ?? y.DateCreated),
                e.Config,
                e.NameShort
            }).FirstOrDefaultAsync(e => e.NameShort == boardName);

            var count = board.Threads.Count();
            var threads = board.Threads.ToArray();

            //this is to ensure that if for some reason we ever lower the maximum thread
            //count of a board that it will automatically cull the threads until it gets
            //down to the required amount
            if (count >= board.Config.MaximumThreadCount)
            {
                for (int i = 0; i <= count - board.Config.MaximumThreadCount; i++)
                {
                    threads[i].IsArchived = true;
                    _context.Posts.Update(threads[i]);
                }
            }
        }

        public async Task<bool> IsChecksumUnique(string checksum, string boardName, int threadId)
        {
            var board = await _context.Boards.Select(e => new BoardDTO()
            {
                NameShort = e.NameShort,
                CurrentThread = e.Threads.Select(y => new PostDTO(){ 
                    Id = y.IdEffective,
                    ImageChecksum = y.ImageChecksum,
                    Posts = y.Posts.Where(x => x.ImageChecksum == checksum)
                        .Select(x => new PostDTO()
                        {
                            ImageChecksum = x.ImageChecksum
                        })
                    }).FirstOrDefault(y => y.Id == threadId)
            }).FirstOrDefaultAsync(e => e.NameShort == boardName);

            if (board?.CurrentThread == null) throw new NullReferenceException();

            return board.CurrentThread.ImageChecksum == checksum || board.CurrentThread.Posts.Any() ? false : true;
        }

        public async Task<BoardDTO> GetPost(string boardName, int postId)
        {
            return await _context.Boards.Select(e => new BoardDTO()
            {
                Id = e.Id,
                DateCreated = e.DateCreated,
                DateEdited = e.DateEdited,
                Name = e.Name,
                NameShort = e.NameShort,
                CurrentThread = e.Threads.Select(y => new PostDTO()
                {
                    Content = y.Content,
                    BumpDate = y.BumpDate,
                    DateCreated = y.DateCreated,
                    DateEdited = y.DateEdited,
                    Id = y.IdEffective,
                    IsOp = y.IsOp,
                    ThreadId = !y.IsOp ? (int?)y.Thread.IdEffective : null,
                    BoardId = y.BoardId,
                    ImageUrl = y.ImageUrl,
                    ThumbnailUrl = y.ThumbnailUrl
                }).FirstOrDefault(y => y.Id == postId && !y.IsArchived)
            }).FirstOrDefaultAsync(e => e.NameShort == boardName);
        }
    }
}
