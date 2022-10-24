using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoHosting.Domain.Entities;
using VideoHosting.Domain.Infrastructure;

namespace VideoHosting.DataBase
{
    public class DataBaseContext : IdentityDbContext<User>
    {
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<Video> Videos { get; set; }

        public virtual DbSet<Commentary> Commentaries { get; set; }

        public virtual DbSet<AppSwitch> AppSwitches { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
             : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server = .\SQLEXPRESS; Database = VideoHostingCore; Trusted_Connection=True;");
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserUser>()
                .HasKey(x => new { x.SubscriberId, x.SubscripterId });
            builder.Entity<VideoUser>()
                .HasKey(x => new { x.UserId, x.VideoId });

            //builder.Entity<User>()
            //    .HasKey(x => new {x.Id})
            //    .IsClustered();

            builder.Entity<User>()
                .HasMany(x => x.Videos)
                .WithOne(x => x.User);

            builder.Entity<User>()
                .HasMany(x => x.Commentaries)
                .WithOne(x => x.User);

            builder.Entity<User>()
                .HasMany(x => x.Subscribers)
                .WithOne(x => x.Subscripter)
                .HasForeignKey(x => x.SubscripterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasMany(x => x.Subscriptions)
                .WithOne(x => x.Subscriber)
                .HasForeignKey(x => x.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<VideoUser>()
            //    .HasOne(x => x.User)
            //    .WithMany(x => x.Dislikes)
            //    .HasForeignKey(x => x.UserId);
            //builder.Entity<VideoUser>()
            //    .HasOne(x => x.Video)
            //    .WithMany(x => x.Dislikes)
            //    .HasForeignKey(x => x.VideoId);

            builder.Entity<User>()
                .HasMany(x => x.Reactions)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Entity<Video>()
                .HasMany(x => x.Commentaries)
                .WithOne(x => x.Video)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Video>()
               .HasMany(x => x.Reactions)
               .WithOne(x => x.Video)
               .HasForeignKey(x=>x.VideoId)
               .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<Video>()
            //   .HasMany(x => x.Dislikes)
            //   .WithOne(x => x.Video)
            //   .HasForeignKey(x => x.VideoId); 

            builder.Entity<AppSwitch>()
                .HasKey(x => x.Key);

            builder.Entity<UserRole>().HasData(
                new UserRole[]
                {
                    new UserRole { Name = "Admin", Description = "Can delete video, users, commentaries", NormalizedName = "ADMIN" },
                    new UserRole { Name = "User", Description = "Can add video, commentaries, likes", NormalizedName = "USER" },
                    new UserRole { Name = "MainAdmin", Description = "Can do everything that admin and can add,delete them", NormalizedName = "MAINADMIN" }
                });


            base.OnModelCreating(builder);
        }
    }
}
