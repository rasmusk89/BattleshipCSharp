﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int?>("GameOptionId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerAId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerBId")
                        .HasColumnType("int");

                    b.HasKey("GameId");

                    b.HasIndex("GameOptionId");

                    b.HasIndex("PlayerAId");

                    b.HasIndex("PlayerBId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.GameOption", b =>
                {
                    b.Property<int>("GameOptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoardHeight")
                        .HasColumnType("int");

                    b.Property<int>("BoardWidth")
                        .HasColumnType("int");

                    b.Property<int>("EShipsCanTouch")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("NextMoveAfterHit")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfShips")
                        .HasColumnType("int");

                    b.HasKey("GameOptionId");

                    b.ToTable("GameOptions");
                });

            modelBuilder.Entity("Domain.GameShip", b =>
                {
                    b.Property<int>("GameShipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ECellState")
                        .HasColumnType("int");

                    b.Property<int>("Hits")
                        .HasColumnType("int");

                    b.Property<bool>("IsSunk")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("GameShipId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GameShips");
                });

            modelBuilder.Entity("Domain.GameState", b =>
                {
                    b.Property<int>("GameStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("GameId")
                        .HasColumnType("int");

                    b.Property<bool>("NextMoveByPlayerA")
                        .HasColumnType("bit");

                    b.Property<string>("PlayerABoardState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerBBoardState")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameStateId");

                    b.HasIndex("GameId");

                    b.ToTable("GameStates");
                });

            modelBuilder.Entity("Domain.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("PlayerType")
                        .HasColumnType("int");

                    b.HasKey("PlayerId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.GameOption", "GameOption")
                        .WithMany()
                        .HasForeignKey("GameOptionId");

                    b.HasOne("Domain.Player", "PlayerA")
                        .WithMany()
                        .HasForeignKey("PlayerAId");

                    b.HasOne("Domain.Player", "PlayerB")
                        .WithMany()
                        .HasForeignKey("PlayerBId");

                    b.Navigation("GameOption");

                    b.Navigation("PlayerA");

                    b.Navigation("PlayerB");
                });

            modelBuilder.Entity("Domain.GameShip", b =>
                {
                    b.HasOne("Domain.Player", "Player")
                        .WithMany("GameShips")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Domain.GameState", b =>
                {
                    b.HasOne("Domain.Game", "Game")
                        .WithMany("GameStates")
                        .HasForeignKey("GameId");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Navigation("GameStates");
                });

            modelBuilder.Entity("Domain.Player", b =>
                {
                    b.Navigation("GameShips");
                });
#pragma warning restore 612, 618
        }
    }
}
