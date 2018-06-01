using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using Forum020.Data.Entities;
using Newtonsoft.Json;
using System.IO;

namespace Forum020.Data
{
    public static class DbContextExtensions
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeeded(this ForumContext context)
        {
            if (!context.Boards.Any())
            {
                Seed.SeedDb(context);

                //var boards =
                //    JsonConvert.DeserializeObject<List<Board>>(
                //        File.ReadAllText("seed" + Path.DirectorySeparatorChar + "seed.json"));
                //foreach (var board in boards)
                //{
                //    context.Boards.Add(board);
                //}
                //context.SaveChanges();
            }
        }
    }
}
