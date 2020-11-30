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

            modelBuilder.Entity("Domain.PlayerBoardState", b =>
                {
                    b.Property<int>("PlayerBoardStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("GameBoardState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.HasKey("PlayerBoardStateId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerBoardStates");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.GameOption", "GameOption")
                        .WithMany("Games")
                        .HasForeignKey("GameOptionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Player", "PlayerA")
                        .WithMany()
                        .HasForeignKey("PlayerAId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Player", "PlayerB")
                        .WithMany()
                        .HasForeignKey("PlayerBId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("GameOption");

                    b.Navigation("PlayerA");

                    b.Navigation("PlayerB");
                });

            modelBuilder.Entity("Domain.GameShip", b =>
                {
                    b.HasOne("Domain.Player", "Player")
                        .WithMany("GameShips")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Domain.PlayerBoardState", b =>
                {
                    b.HasOne("Domain.Player", "Player")
                        .WithMany("PlayerBoardStates")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Domain.GameOption", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("Domain.Player", b =>
                {
                    b.Navigation("GameShips");

                    b.Navigation("PlayerBoardStates");
                });
#pragma warning restore 612, 618
        }
    }
}
