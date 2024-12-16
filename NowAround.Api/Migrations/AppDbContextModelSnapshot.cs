﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NowAround.Api.Database;

#nullable disable

namespace NowAround.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryEstablishment", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<int>("EstablishmentsId")
                        .HasColumnType("int");

                    b.HasKey("CategoriesId", "EstablishmentsId");

                    b.HasIndex("EstablishmentsId");

                    b.ToTable("CategoryEstablishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHours", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Friday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Monday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Saturday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Sunday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Thursday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Tuesday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("Wednesday")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId")
                        .IsUnique();

                    b.ToTable("BusinessHours");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHoursException", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BusinessHoursId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BusinessHoursId");

                    b.ToTable("BusinessHoursExceptions");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Cuisine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Cuisines");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Establishment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Auth0Id")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("PriceCategory")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Website")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique();

                    b.ToTable("Establishments");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.EstablishmentCuisine", b =>
                {
                    b.Property<int>("CuisineId")
                        .HasColumnType("int");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.HasKey("CuisineId", "EstablishmentId");

                    b.HasIndex("CuisineId");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("EstablishmentCuisines");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.EstablishmentTag", b =>
                {
                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("EstablishmentId", "TagId");

                    b.HasIndex("EstablishmentId");

                    b.HasIndex("TagId");

                    b.ToTable("EstablishmentTags");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("UserFriendId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserFriendId");

                    b.HasIndex("UserId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.FriendRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("MenuId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("PhotoUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.MonthlyStatistic", b =>
                {
                    b.Property<string>("Date")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EstablishmentsCreatedCount")
                        .HasColumnType("int");

                    b.Property<int>("UsersCreatedCount")
                        .HasColumnType("int");

                    b.HasKey("Date");

                    b.ToTable("MonthlyStatistics");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.PostLike", b =>
                {
                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PostId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("PostLikes");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.RatingStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<int>("FiveStars")
                        .HasColumnType("int");

                    b.Property<int>("FourStars")
                        .HasColumnType("int");

                    b.Property<int>("OneStar")
                        .HasColumnType("int");

                    b.Property<int>("ThreeStars")
                        .HasColumnType("int");

                    b.Property<int>("TwoStars")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId")
                        .IsUnique();

                    b.ToTable("RatingStatistics");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("RatingCollectionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RatingCollectionId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.SocialLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("SocialLinks");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Auth0Id")
                        .IsRequired()
                        .HasMaxLength(48)
                        .HasColumnType("nvarchar(48)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CategoryEstablishment", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.Establishment", null)
                        .WithMany()
                        .HasForeignKey("EstablishmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHours", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithOne("BusinessHours")
                        .HasForeignKey("NowAround.Api.Models.Domain.BusinessHours", "EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHoursException", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.BusinessHours", "BusinessHours")
                        .WithMany("BusinessHoursExceptions")
                        .HasForeignKey("BusinessHoursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BusinessHours");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.EstablishmentCuisine", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Cuisine", "Cuisine")
                        .WithMany("EstablishmentCuisines")
                        .HasForeignKey("CuisineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("EstablishmentCuisines")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Cuisine");

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.EstablishmentTag", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("EstablishmentTags")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.Tag", "Tag")
                        .WithMany("EstablishmentTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Friend", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.User", "UserFriend")
                        .WithMany()
                        .HasForeignKey("UserFriendId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserFriend");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.FriendRequest", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Menu", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("Menus")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.MenuItem", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Menu", "Menu")
                        .WithMany("MenuItems")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Menu");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Post", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("Posts")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.PostLike", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Post", "Post")
                        .WithMany("PostLikes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", "User")
                        .WithMany("PostLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.RatingStatistic", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithOne("RatingStatistic")
                        .HasForeignKey("NowAround.Api.Models.Domain.RatingStatistic", "EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Review", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.RatingStatistic", "RatingStatistic")
                        .WithMany("Reviews")
                        .HasForeignKey("RatingCollectionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RatingStatistic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.SocialLink", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("SocialLinks")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHours", b =>
                {
                    b.Navigation("BusinessHoursExceptions");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Cuisine", b =>
                {
                    b.Navigation("EstablishmentCuisines");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Establishment", b =>
                {
                    b.Navigation("BusinessHours")
                        .IsRequired();

                    b.Navigation("EstablishmentCuisines");

                    b.Navigation("EstablishmentTags");

                    b.Navigation("Menus");

                    b.Navigation("Posts");

                    b.Navigation("RatingStatistic")
                        .IsRequired();

                    b.Navigation("SocialLinks");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Menu", b =>
                {
                    b.Navigation("MenuItems");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Post", b =>
                {
                    b.Navigation("PostLikes");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.RatingStatistic", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Tag", b =>
                {
                    b.Navigation("EstablishmentTags");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.User", b =>
                {
                    b.Navigation("Friends");

                    b.Navigation("PostLikes");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
