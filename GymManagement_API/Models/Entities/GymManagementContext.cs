using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Models.Entities;

public partial class GymManagementContext : DbContext
{
    public GymManagementContext()
    {
    }

    public GymManagementContext(DbContextOptions<GymManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberPackage> MemberPackages { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(local); Initial Catalog=Gym_Management;Persist Security Info=True;User ID=sa;Password=Hieu1309;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__4A311D2F5087AF8A");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Email, "UQ__Admin__A9D105340CB43D54").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Admin__C9F284569A7E5BF0").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("Admin_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__20D6A968EC323DC6");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("attendance_id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.ClassId).HasColumnName("Class_Id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");

            entity.HasOne(d => d.Class).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Attendanc__Class__5629CD9C");

            entity.HasOne(d => d.Member).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Attendanc__membe__5535A963");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__B09705377A7AB106");

            entity.Property(e => e.ClassId).HasColumnName("Class_Id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Class_Name");
            entity.Property(e => e.MemberId).HasColumnName("Member_Id");
            entity.Property(e => e.TrainerId).HasColumnName("Trainer_Id");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__Classes__Class_N__52593CB8");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__CDC95E708D4B1036");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("Feedback_id");
            entity.Property(e => e.Feedback1)
                .HasColumnType("text")
                .HasColumnName("feedback");
            entity.Property(e => e.FeedbackDate).HasColumnName("feedback_date");
            entity.Property(e => e.TrainerId).HasColumnName("trainer_id");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__Feedback__traine__59063A47");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__42A18B6FD0F9A710");

            entity.ToTable("Member");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Member__85FB4E38EF4C86CA").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Member__A9D105349B7D1082").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Member__C9F284560C1D18A5").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("Member_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirtName).HasMaxLength(15);
            entity.Property(e => e.LastName).HasMaxLength(15);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserImg)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("User_img");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MemberPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Member_P__3214EC2711816CA6");

            entity.ToTable("Member_Package");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.PackageId).HasColumnName("Package_ID");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberPackages)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Member_Pa__membe__44FF419A");

            entity.HasOne(d => d.Package).WithMany(p => p.MemberPackages)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Member_Pa__Packa__45F365D3");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Packages__B7FDB5328B42A09C");

            entity.HasIndex(e => e.PackageName, "UQ__Packages__6AB969E3ED7031E6").IsUnique();

            entity.Property(e => e.PackageId)
                .ValueGeneratedNever()
                .HasColumnName("Package_id");
            entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Duration)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PackageName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Package_name");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__DA6C7FC111398863");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Payment_Id");
            entity.Property(e => e.CardName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("card_Name");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("card_Number");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");

            entity.HasOne(d => d.PaymentNavigation).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Payment__48CFD27E");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PK__Trainer__8B0DBD69B9B0B008");

            entity.ToTable("Trainer");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Trainer__85FB4E38A3530769").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Trainer__A9D10534B883CD38").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Trainer__C9F28456AB878290").IsUnique();

            entity.Property(e => e.TrainerId).HasColumnName("Trainer_id");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirtName).HasMaxLength(15);
            entity.Property(e => e.LastName).HasMaxLength(15);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserImg)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("User_img");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
