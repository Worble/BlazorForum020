﻿// <auto-generated />
using System;
using Forum020.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Forum020.Server.Migrations
{
    [DbContext(typeof(ForumContext))]
    [Migration("20180609232044_make delete behaviour cascade")]
    partial class makedeletebehaviourcascade
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Forum020.Data.Entities.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConfigId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateEdited");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("NameShort")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ConfigId")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("NameShort")
                        .IsUnique();

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Forum020.Data.Entities.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateEdited");

                    b.Property<int>("MaximumReplyCount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(100);

                    b.Property<int>("MaximumThreadCount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(12);

                    b.HasKey("Id");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("Forum020.Data.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BoardId");

                    b.Property<DateTime?>("BumpDate");

                    b.Property<string>("Content");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateEdited");

                    b.Property<int>("IdEffective")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageChecksum");

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsArchived");

                    b.Property<bool>("IsOp");

                    b.Property<int?>("ThreadId");

                    b.Property<string>("ThumbnailUrl");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.HasIndex("ThreadId");

                    b.HasIndex("IdEffective", "BoardId")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Forum020.Data.Entities.Board", b =>
                {
                    b.HasOne("Forum020.Data.Entities.Config", "Config")
                        .WithOne()
                        .HasForeignKey("Forum020.Data.Entities.Board", "ConfigId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Forum020.Data.Entities.Post", b =>
                {
                    b.HasOne("Forum020.Data.Entities.Board", "Board")
                        .WithMany("Threads")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Forum020.Data.Entities.Post", "Thread")
                        .WithMany("Posts")
                        .HasForeignKey("ThreadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
