﻿// <auto-generated />
using System;
using Leopard.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    [DbContext(typeof(OneContext))]
    [Migration("20200531070643_seed-data-for-work")]
    partial class seeddataforwork
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Leopard.Domain.AdminRequestAG.AdminRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("uuid");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("AdminRequests");
                });

            modelBuilder.Entity("Leopard.Domain.AttitudeAG.Attitude", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Agree")
                        .HasColumnType("boolean");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Attitudes");
                });

            modelBuilder.Entity("Leopard.Domain.CommentAG.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AgreeCount")
                        .HasColumnType("integer");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<int>("DisagreeCount")
                        .HasColumnType("integer");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("WorkId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Leopard.Domain.DiscussionAG.Discussion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("TextTsv")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasComputedColumnSql("to_tsvector('testzhcfg',coalesce(\"Text\",'')) ");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("uuid");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TextTsv")
                        .HasAnnotation("Npgsql:IndexMethod", "GIN");

                    b.ToTable("Discussions");
                });

            modelBuilder.Entity("Leopard.Domain.PostAG.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsEssence")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPinned")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("uuid");

                    b.Property<NpgsqlTsVector>("Tsv")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Title"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Text"",'')), 'B') ");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Tsv")
                        .HasAnnotation("Npgsql:IndexMethod", "GIN");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Leopard.Domain.ReplyAG.Reply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("TextTsv")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasComputedColumnSql(" to_tsvector('testzhcfg',coalesce(\"Text\",'')) ");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TextTsv")
                        .HasAnnotation("Npgsql:IndexMethod", "GIN");

                    b.ToTable("Replies");
                });

            modelBuilder.Entity("Leopard.Domain.ReportAG.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<bool>("Handled")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Leopard.Domain.TopicAG.Topic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsGroup")
                        .HasColumnType("boolean");

                    b.Property<int>("MemberCount")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("RelatedWork")
                        .HasColumnType("uuid");

                    b.Property<NpgsqlTsVector>("Tsv")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Tsv")
                        .HasAnnotation("Npgsql:IndexMethod", "GIN");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("Leopard.Domain.TopicMemberAG.TopicMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("uuid");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("TopicMembers");
                });

            modelBuilder.Entity("Leopard.Domain.UserAG.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<int>("SecurityVersion")
                        .HasColumnType("integer");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Leopard.Domain.WorkAG.Work", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("CoverUrl")
                        .HasColumnType("text");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("Tsv")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasComputedColumnSql(@"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Author"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<long>("UpdatedAt")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Tsv")
                        .HasAnnotation("Npgsql:IndexMethod", "GIN");

                    b.ToTable("Works");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0a180b32-2510-406f-b0fd-1e19b6bb2697"),
                            Author = "some author",
                            CoverUrl = "https://i.loli.net/2020/05/31/prCLIHej56ZMOwo.jpg",
                            CreatedAt = 1590908802534L,
                            Description = "description hello",
                            Name = "testbook1",
                            Type = 0,
                            UpdatedAt = 1590908802534L
                        },
                        new
                        {
                            Id = new Guid("71a710dc-aab6-4b9e-97eb-61ae1d703117"),
                            Author = "some director",
                            CoverUrl = "https://i.loli.net/2020/05/31/prCLIHej56ZMOwo.jpg",
                            CreatedAt = 1590908802542L,
                            Description = "description hello",
                            Name = "testfilm1",
                            Type = 1,
                            UpdatedAt = 1590908802542L
                        });
                });

            modelBuilder.Entity("Leopard.Infrastructure.DeduplicationToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Additional")
                        .HasColumnType("text");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<string>("HandlerType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EventId", "HandlerType", "Additional")
                        .IsUnique();

                    b.ToTable("DeduplicationTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
