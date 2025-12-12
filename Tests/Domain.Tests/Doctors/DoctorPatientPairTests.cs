using Domain.Doctors;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Doctors;

public sealed class DoctorPatientPairTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var link = DoctorPatientPair.Create(id, doctorId, patientId);

        link.Id.Should().Be(id);
        link.DoctorUserId.Should().Be(doctorId);
        link.PatientUserId.Should().Be(patientId);
    }
}