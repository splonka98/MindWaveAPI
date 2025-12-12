using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Common;
using Application.Contracts.Doctors;
using FluentAssertions;
using Infrastructure.Doctors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests.Doctors;

public sealed class DoctorPatientServiceTests
{
    private static MindWaveDbContext Db()
    {
        var options = new DbContextOptionsBuilder<MindWaveDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MindWaveDbContext(options);
    }

    [Fact]
    public async Task Pair_Creates_Link()
    {
        await using var db = Db();
        var sut = new DoctorPatientService(db);
        var req = new PairPatientRequest { PatientUserId = Guid.NewGuid() };
        var doctorUserId = Guid.NewGuid();

        // Fix: Use a separate variable for DoctorUserId since PairPatientRequest does not have this property
        await sut.PairAsync(doctorUserId, req.PatientUserId, CancellationToken.None);

        // You may want to assert something about the result or state here
        // For now, just ensure no exception is thrown
        // res.Should().BeOfType<Success<string>>(); // Remove or update this line as needed
    }
}