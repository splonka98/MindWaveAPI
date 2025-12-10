using Domain.Surveys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class SurveyAnswerConfiguration : IEntityTypeConfiguration<SurveyAnswer>
{
    public void Configure(EntityTypeBuilder<SurveyAnswer> builder)
    {
        builder.ToTable("survey_answers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SurveyInstanceId).IsRequired();
        builder.Property(x => x.PatientUserId).IsRequired();
        builder.Property(x => x.Date).HasColumnType("date").IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.Value).IsRequired();

        builder.HasIndex(x => new { x.PatientUserId, x.Date });
        builder.HasIndex(x => new { x.SurveyInstanceId, x.QuestionId }).IsUnique();
    }
}