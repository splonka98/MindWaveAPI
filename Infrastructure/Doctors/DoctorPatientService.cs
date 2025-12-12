using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Doctors;
using Application.Contracts.Doctors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Doctors;

public sealed class DoctorPatientService : IDoctorPatientService
{
    private readonly MindWaveDbContext _db;

    public DoctorPatientService(MindWaveDbContext db) => _db = db;

    public async Task PairAsync(Guid doctorUserId, Guid patientUserId, CancellationToken ct)
    {
        var exists = await _db.DoctorPatientPairs.AnyAsync(p => p.DoctorUserId == doctorUserId && p.PatientUserId == patientUserId, ct);
        if (exists) return;

        var pair = Domain.Doctors.DoctorPatientPair.Create(Guid.NewGuid(), doctorUserId, patientUserId);
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

    public async Task<IReadOnlyList<PatientSurveyWithAnswersDto>> GetPatientSurveysWithAnswersAsync(Guid doctorUserId, Guid patientUserId, DateOnly from, DateOnly to, CancellationToken ct)
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

        var results = new List<PatientSurveyWithAnswersDto>(instances.Count);

        foreach (var inst in instances)
        {
            var allAnswers = await _db.SurveyAnswers
                .Where(a => a.SurveyInstanceId == inst.Id)
                .OrderBy(a => a.QuestionId)
                .ToListAsync(ct);

            // Wylicz kategorię z odpowiedzi inicjalnych
            var initAnswers = allAnswers.Where(a => a.QuestionId is >= 1 and <= 4).ToList();
            if (initAnswers.Count != 4)
            {
                // Jeśli brak kompletu inicjalnych, pomiń tę instancję
                continue;
            }

            var sum = initAnswers.Sum(a => a.Value);
            var category = sum < 16 ? "depression" : (sum < 32 ? "hypomania" : "mania");

            results.Add(new PatientSurveyWithAnswersDto
            {
                SurveyInstanceId = inst.Id,
                Date = inst.Date,
                Category = category,
                Answers = allAnswers
                    .Select(a => new PatientSurveyWithAnswersDto.SurveyAnswerDto
                    {
                        QuestionId = a.QuestionId,
                        Value = a.Value
                    })
                    .ToList()
            });
        }

        return results;
    }
}