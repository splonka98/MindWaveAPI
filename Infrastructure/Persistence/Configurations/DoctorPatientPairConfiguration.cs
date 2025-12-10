using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class DoctorPatientPairConfiguration : IEntityTypeConfiguration<DoctorPatientPair>
{
    public void Configure(EntityTypeBuilder<DoctorPatientPair> builder)
    {
        builder.ToTable("doctor_patient_pairs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DoctorUserId).IsRequired();
        builder.Property(x => x.PatientUserId).IsRequired();
        builder.HasIndex(x => new { x.DoctorUserId, x.PatientUserId }).IsUnique();
    }
}