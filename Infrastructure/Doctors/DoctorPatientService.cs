using Application.Abstractions.Doctors;
using Application.Contracts.Doctors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Doctors;
using Application.Contracts.Surveys;

namespace Infrastructure.Doctors;

public sealed class DoctorPatientService : IDoctorPatientService
{
    private readonly MindWaveDbContext _db;

    public DoctorPatientService(MindWaveDbContext db) => _db = db;

    public async Task PairAsync(Guid doctorUserId, Guid patientUserId, CancellationToken ct)
    {
        var exists = await _db.DoctorPatientPairs.AnyAsync(p => p.DoctorUserId == doctorUserId && p.PatientUserId == patientUserId, ct);
        if (exists) return;

        var pair = DoctorPatientPair.Create(Guid.NewGuid(), doctorUserId, patientUserId);
        _db.DoctorPatientPairs.Add(pair);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PatientSurveyDto>> GetPatientSurveysAsync(Guid doctorUserId, Guid patientUserId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var allowed = await _db.DoctorPatientPairs.AnyAsync(p => p.DoctorUserId == doctorUserId && p.PatientUserId == patientUserId, ct);
        if (!allowed)
        {
            throw new UnauthorizedAccessException("Doctor is not paired with this patient.");
        }

        var instances = await _db.SurveyInstances
            .Where(s => s.PatientUserId == patientUserId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ToListAsync(ct);

        var results = new List<PatientSurveyDto>(instances.Count);
        foreach (var inst in instances)
        {
            var initAnswers = await _db.SurveyAnswers
                .Where(a => a.SurveyInstanceId == inst.Id && a.QuestionId >= 1 && a.QuestionId <= 4)
                .OrderBy(a => a.QuestionId)
                .ToListAsync(ct);

            if (initAnswers.Count != 4)
            {
                continue;
            }

            var sum = initAnswers.Sum(a => a.Value);
            var category = sum < 16 ? "depression" : (sum < 32 ? "hypomania" : "mania");

            results.Add(new PatientSurveyDto
            {
                SurveyInstanceId = inst.Id,
                Date = inst.Date,
                Category = category
            });
        }

        return results;
    }
}