using Domain.Users;
using Domain.Pairing;
using Domain.Surveys;
using Domain.Analytics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class MindWaveDbContext : DbContext
{
    public MindWaveDbContext(DbContextOptions<MindWaveDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SurveyInstance> SurveyInstances => Set<SurveyInstance>();
    public DbSet<PairingToken> PairingTokens => Set<PairingToken>();
    public DbSet<DoctorPatientLink> DoctorPatientLinks => Set<DoctorPatientLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).IsRequired().HasMaxLength(256);
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            b.Property(u => u.Role).IsRequired().HasMaxLength(32);
            b.Property(u => u.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<SurveyInstance>(b =>
        {
            b.ToTable("survey_instances");
            b.HasKey(s => s.Id);
            b.HasIndex(s => new { s.PatientUserId, s.Date }).IsUnique();
            b.Property(s => s.TemplateId).IsRequired();
            b.Property(s => s.Date).IsRequired();

            b.OwnsMany(s => s.Answers, ab =>
            {
                ab.ToTable("survey_answers");
                ab.WithOwner().HasForeignKey("survey_instance_id");
                ab.Property<int>("question_id");
                ab.Property<string>("value").IsRequired().HasMaxLength(1024);
                ab.HasKey("survey_instance_id", "question_id");
            });

            b.Ignore(s => s.Tags);
        });

        modelBuilder.Entity<PairingToken>().HasKey(t => t.Id);
        modelBuilder.Entity<PairingToken>()
            .HasIndex(t => t.Code)
            .IsUnique();
        modelBuilder.Entity<PairingToken>()
            .Property(t => t.Code)
            .IsRequired().HasMaxLength(32);

        modelBuilder.Entity<DoctorPatientLink>().HasKey(l => l.Id);
        modelBuilder.Entity<DoctorPatientLink>()
            .Property(l => l.DoctorUserId)
            .IsRequired();
        modelBuilder.Entity<DoctorPatientLink>()
            .Property(l => l.PatientUserId)
            .IsRequired();
    }
}