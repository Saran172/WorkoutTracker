using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Exercise = Shared.Entities.Exercise;

namespace WorkoutTracker.API.DbCon
{
    public class WTContext : DbContext
    {
        public WTContext(DbContextOptions<WTContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<DevicePushToken> DevicePushTokens { get; set; }
        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public virtual DbSet<WorkoutSessionExercise> WorkoutSessionExercises { get; set; }
        public virtual DbSet<WorkoutSessionSet> WorkoutSessionSets { get; set; }
        public DbSet<WorkoutTemplate> WorkoutTemplates => Set<WorkoutTemplate>();
        public DbSet<WorkoutTemplateExercise> WorkoutTemplateExercises => Set<WorkoutTemplateExercise>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToTable("Users");
            modelBuilder.Entity<UserDevice>()
                .ToTable("UserDevices");
            modelBuilder.Entity<DevicePushToken>()
                .ToTable("DevicePushTokens");
            modelBuilder.Entity<Exercise>()
                .ToTable("Exercises");


            modelBuilder.Entity<WorkoutSession>(e =>
            {
                e.ToTable("WorkoutSessions");
                e.Property(x => x.PublicId)
                 .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("SYSUTCDATETIME()");

                e.Property(x => x.Version)
                 .HasDefaultValue(1);

                e.Property(x => x.Status)
                 .HasDefaultValue("Completed");
            });

            modelBuilder.Entity<WorkoutSessionExercise>(e =>
            {
                e.ToTable("WorkoutExercises");
                e.Property(x => x.PublicId)
                 .HasDefaultValueSql("NEWSEQUENTIALID()");
            });

            modelBuilder.Entity<WorkoutSessionSet>(e =>
            {
                e.ToTable("WorkoutSets");
                e.Property(x => x.PublicId)
                 .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            ConfigureWorkoutTemplates(modelBuilder);
            ConfigureWorkoutTemplateExercises(modelBuilder);
        }

        private static void ConfigureWorkoutTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkoutTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PublicId)
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSUTCDATETIME()");
            });
        }

        private static void ConfigureWorkoutTemplateExercises(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkoutTemplateExercise>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSUTCDATETIME()");
            });
        }


    }
}
