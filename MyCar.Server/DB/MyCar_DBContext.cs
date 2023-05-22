using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyCar.Server.DB
{
    public partial class MyCar_DBContext : DbContext
    {
        public MyCar_DBContext()
        {
        }

        public MyCar_DBContext(DbContextOptions<MyCar_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionType> ActionTypes { get; set; } = null!;
        public virtual DbSet<BodyType> BodyTypes { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarPhoto> CarPhotos { get; set; } = null!;
        public virtual DbSet<Characteristic> Characteristics { get; set; } = null!;
        public virtual DbSet<CharacteristicCar> CharacteristicCars { get; set; } = null!;
        public virtual DbSet<CountChangeHistory> CountChangeHistories { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<Equipment> Equipment { get; set; } = null!;
        public virtual DbSet<MarkCar> MarkCars { get; set; } = null!;
        public virtual DbSet<Model> Models { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Passport> Passports { get; set; } = null!;
        public virtual DbSet<SaleCar> SaleCars { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserType> UserTypes { get; set; } = null!;
        public virtual DbSet<Warehouse> Warehouses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-CAGO29I\\SQLEXPRESS;Initial Catalog=MyCar_DB1;Trusted_Connection=True; User=dbo");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.ToTable("ActionType");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ActionTypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BodyType>(entity =>
            {
                entity.ToTable("BodyType");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("Car");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Articul).HasMaxLength(50);
                entity.Property(e => e.CarPrice).HasColumnType("money");
                entity.Property(e => e.PhotoCar).IsUnicode(false);
                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.ModelId)
                    .HasConstraintName("FK_Car_Model");
                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Car_BodyType");
            });

            modelBuilder.Entity<CarPhoto>(entity =>
            {
                entity.ToTable("CarPhoto");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.PhotoName).IsUnicode(false);
                entity.HasOne(d => d.SaleCar)
                    .WithMany(p => p.CarPhotos)
                    .HasForeignKey(d => d.SaleCarId)
                    .HasConstraintName("FK_CarPhoto_SaleCar");
            });

            modelBuilder.Entity<Characteristic>(entity =>
            {
                entity.ToTable("Characteristic");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CharacteristicName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Characteristics)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_Characteristic_Unit");
            });

            modelBuilder.Entity<CharacteristicCar>(entity =>
            {
                entity.HasKey(e => new { e.CarId, e.CharacteristicId });
                entity.ToTable("CharacteristicCar");
                entity.Property(e => e.CharacteristicValue).HasMaxLength(50);
                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CharacteristicCars)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CharacteristicCar_Car");
                entity.HasOne(d => d.Characteristic)
                    .WithMany(p => p.CharacteristicCars)
                    .HasForeignKey(d => d.CharacteristicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CharacteristicCar_Characteristic");
            });

            modelBuilder.Entity<CountChangeHistory>(entity =>
            {
                entity.ToTable("CountChangeHistory");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.HasOne(d => d.WarehouseIn)
                    .WithMany(p => p.CountChangeHistoryWarehouseIns)
                    .HasForeignKey(d => d.WarehouseInId)
                    .HasConstraintName("FK_CountChangeHistory_Warehouse1");
                entity.HasOne(d => d.WarehouseOut)
                    .WithMany(p => p.CountChangeHistoryWarehouseOuts)
                    .HasForeignKey(d => d.WarehouseOutId)
                    .HasConstraintName("FK_CountChangeHistory_Warehouse");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 0)");
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.HasOne(d => d.SaleCar)
                    .WithMany(p => p.Discounts)
                    .HasForeignKey(d => d.SaleCarId)
                    .HasConstraintName("FK_Discount_SaleCar");
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.NameEquipment)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MarkCar>(entity =>
            {
                entity.ToTable("MarkCar");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MarkName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.MarkPhoto).IsUnicode(false);
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("Model");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ModelName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.HasOne(d => d.Mark)
                    .WithMany(p => p.Models)
                    .HasForeignKey(d => d.MarkId)
                    .HasConstraintName("FK_Model_MarkCar");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.DateOfOrder).HasColumnType("date");
                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ActionTypeId)
                    .HasConstraintName("FK_Order_ActionType");
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Order_Status");
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<Passport>(entity =>
            {
                entity.ToTable("Passport");
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
                entity.Property(e => e.DateEnd).HasColumnType("date");
                entity.Property(e => e.DateStart).HasColumnType("date");
                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Number)
                    .HasMaxLength(10)
                    .IsFixedLength();
                entity.Property(e => e.Patronymic)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Seria)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SaleCar>(entity =>
            {
                entity.ToTable("SaleCar");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Articul)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.EquipmentPrice).HasColumnType("money");
                entity.HasOne(d => d.Car)
                    .WithMany(p => p.SaleCars)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK_SaleCar_Car");
                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.SaleCars)
                    .HasForeignKey(d => d.EquipmentId)
                    .HasConstraintName("FK_SaleCar_Equipment");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UnitName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.HasOne(d => d.Passport)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.PassportId)
                    .HasConstraintName("FK_User_Passport");
                entity.HasOne(d => d.UserType)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserTypeId)
                    .HasConstraintName("FK_User_UserType");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("UserType");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.TypeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("Warehouse");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Discount).HasColumnType("money");
                entity.Property(e => e.Price).HasColumnType("money");
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Warehouse_Order");
                entity.HasOne(d => d.SaleCar)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.SaleCarId)
                    .HasConstraintName("FK_Warehouse_SaleCar");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}