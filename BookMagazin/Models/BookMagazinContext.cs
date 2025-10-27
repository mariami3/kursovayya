using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookMagazin.Models;

public partial class BookMagazinContext : DbContext
{
    public BookMagazinContext()
    {
    }

    public BookMagazinContext(DbContextOptions<BookMagazinContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adaptation> Adaptations { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookVoice> BookVoices { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<FavoriteItem> FavoriteItems { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=WIN-5O8Q88327DH\\SQLEXPRESS01;Initial Catalog=BookMagazin;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adaptation>(entity =>
        {
            entity.HasKey(e => e.IdAdaptation).HasName("PK__Adaptati__85EFD0DD77139FF8");

            entity.ToTable("Adaptation");

            entity.Property(e => e.IdAdaptation).HasColumnName("ID_Adaptation");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Book).WithMany(p => p.Adaptations)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Adaptatio__Book___59063A47");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.IdAuthor).HasName("PK__Author__83F33C8B14CC78A3");

            entity.ToTable("Author");

            entity.Property(e => e.IdAuthor).HasColumnName("ID_Author");
            entity.Property(e => e.NameAuthor)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.IdBook).HasName("PK__Book__DE8DF0383305806D");

            entity.ToTable("Book");

            entity.Property(e => e.IdBook).HasColumnName("ID_Book");
            entity.Property(e => e.AgeLimit).HasMaxLength(10);
            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.BookType).HasMaxLength(50);
            entity.Property(e => e.BookUrl).IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.GenreId).HasColumnName("Genre_ID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PublisherBrand).HasMaxLength(100);
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Book__Author_ID__534D60F1");

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Book__Genre_ID__52593CB8");
        });

        modelBuilder.Entity<BookVoice>(entity =>
        {
            entity.HasKey(e => e.IdVoice).HasName("PK__BookVoic__939912F854C9BDA3");

            entity.ToTable("BookVoice");

            entity.Property(e => e.IdVoice).HasColumnName("ID_Voice");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.Format)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("mp3");
            entity.Property(e => e.Title).IsUnicode(false);
            entity.Property(e => e.VoiceUrl).IsUnicode(false);

            entity.HasOne(d => d.Book).WithMany(p => p.BookVoices)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BookVoice__Book___29221CFB");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.IdCartItem).HasName("PK__CartItem__B943DA22CFD57457");

            entity.ToTable("CartItem");

            entity.Property(e => e.IdCartItem).HasColumnName("ID_CartItem");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Book).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CartItem__Book_I__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CartItem__User_I__656C112C");
        });

        modelBuilder.Entity<FavoriteItem>(entity =>
        {
            entity.HasKey(e => e.IdFavoriteItem).HasName("PK__Favorite__350677E4BF5251B4");

            entity.ToTable("FavoriteItem");

            entity.Property(e => e.IdFavoriteItem).HasColumnName("ID_FavoriteItem");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Book).WithMany(p => p.FavoriteItems)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__FavoriteI__Book___6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__FavoriteI__User___693CA210");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.IdGenre).HasName("PK__Genre__7B31A83B6A82C3A5");

            entity.ToTable("Genre");

            entity.Property(e => e.IdGenre).HasColumnName("ID_Genre");
            entity.Property(e => e.NameGenre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Orders__EC9FA95531A9EA37");

            entity.Property(e => e.IdOrder).HasColumnName("ID_Order");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.StatusOrders)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("В обработке");
            entity.Property(e => e.TotalSum).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Orders__User_ID__619B8048");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem).HasName("PK__OrderIte__0C9F19CB81944E50");

            entity.ToTable("OrderItem");

            entity.Property(e => e.IdOrderItem).HasColumnName("ID_OrderItem");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderItem__Book___6E01572D");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderItem__Order__6D0D32F4");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Reviews__E39E9647459BE1BF");

            entity.Property(e => e.IdReview).HasColumnName("ID_Review");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__Book_ID__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__User_ID__5EBF139D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Roles__43DCD32D6BA87AC4");

            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.NameRole)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__ED4DE442C2FA9B24");

            entity.Property(e => e.IdUser).HasColumnName("ID_User");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordUser)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ResetToken)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Users__Role_ID__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
