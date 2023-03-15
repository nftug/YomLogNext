﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YomLog.Infrastructure;

#nullable disable

namespace YomLog.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("YomLog.Infrastructure.EDMs.AuthorEDM", b =>
                {
                    b.Property<long>("PK")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedByName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("PK");

                    b.HasIndex("Id");

                    b.ToTable("Author", (string)null);
                });

            modelBuilder.Entity("YomLog.Infrastructure.EDMs.BookAuthorEDM", b =>
                {
                    b.Property<long>("PK")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("FKAuthor")
                        .HasColumnType("INTEGER");

                    b.Property<long>("FKBook")
                        .HasColumnType("INTEGER");

                    b.HasKey("PK");

                    b.HasIndex("FKAuthor");

                    b.HasIndex("FKBook");

                    b.ToTable("BookAuthorEDM");
                });

            modelBuilder.Entity("YomLog.Infrastructure.EDMs.BookEDM", b =>
                {
                    b.Property<long>("PK")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookType")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("GoogleBooksId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GoogleBooksUrl")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Isbn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TotalKindleLocation")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TotalPage")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedByName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("PK");

                    b.HasIndex("Id");

                    b.ToTable("Book", (string)null);
                });

            modelBuilder.Entity("YomLog.Infrastructure.EDMs.BookAuthorEDM", b =>
                {
                    b.HasOne("YomLog.Infrastructure.EDMs.AuthorEDM", "Author")
                        .WithMany()
                        .HasForeignKey("FKAuthor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YomLog.Infrastructure.EDMs.BookEDM", "Book")
                        .WithMany("BookAuthors")
                        .HasForeignKey("FKBook")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("YomLog.Infrastructure.EDMs.BookEDM", b =>
                {
                    b.Navigation("BookAuthors");
                });
#pragma warning restore 612, 618
        }
    }
}
