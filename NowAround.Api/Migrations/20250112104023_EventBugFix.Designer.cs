﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NowAround.Api.Database;

#nullable disable

namespace NowAround.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250112104023_EventBugFix")]
    partial class EventBugFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryEstablishment", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId", "EstablishmentId");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("CategoryEstablishment");
                });

            modelBuilder.Entity("EventInterest", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("EventId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("EventInterest");
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

                    b.Property<string>("BackgroundPictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
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

                    b.Property<string>("ProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique();

                    b.ToTable("Establishments");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<int>("EventCategory")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(4);

                    b.Property<string>("EventPriceCategory")
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("MaxParticipants")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Price")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("Event");
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

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

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
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("MenuId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

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
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("Posts");
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
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

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

                    b.Property<string>("BackgroundPictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("ProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PostLike", b =>
                {
                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PostId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("PostLike");
                });

            modelBuilder.Entity("TagEstablishment", b =>
                {
                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("EstablishmentId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("TagEstablishment");
                });

            modelBuilder.Entity("CategoryEstablishment", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.Establishment", null)
                        .WithMany()
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EventInterest", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Event", null)
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
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

            modelBuilder.Entity("NowAround.Api.Models.Domain.Event", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", "Establishment")
                        .WithMany("Events")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Establishment");
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
                        .OnDelete(DeleteBehavior.Cascade)
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

            modelBuilder.Entity("PostLike", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Post", null)
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TagEstablishment", b =>
                {
                    b.HasOne("NowAround.Api.Models.Domain.Establishment", null)
                        .WithMany()
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NowAround.Api.Models.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.BusinessHours", b =>
                {
                    b.Navigation("BusinessHoursExceptions");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.Establishment", b =>
                {
                    b.Navigation("BusinessHours")
                        .IsRequired();

                    b.Navigation("Events");

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

            modelBuilder.Entity("NowAround.Api.Models.Domain.RatingStatistic", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("NowAround.Api.Models.Domain.User", b =>
                {
                    b.Navigation("Friends");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
