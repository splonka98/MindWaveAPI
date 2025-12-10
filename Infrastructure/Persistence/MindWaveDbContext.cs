using Domain.Surveys;
using Domain.Users;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class MindWaveDbContext : DbContext
{
    public MindWaveDbContext(DbContextOptions<MindWaveDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SurveyTemplate> SurveyTemplates => Set<SurveyTemplate>();
    public DbSet<SurveyInstance> SurveyInstances => Set<SurveyInstance>();
    public DbSet<SurveyAnswer> SurveyAnswers => Set<SurveyAnswer>();
    public DbSet<DoctorPatientPair> DoctorPatientPairs => Set<DoctorPatientPair>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MindWaveDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}