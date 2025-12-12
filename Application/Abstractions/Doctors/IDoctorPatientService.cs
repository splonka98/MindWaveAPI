using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Doctors;

namespace Application.Abstractions.Doctors;

public interface IDoctorPatientService
{
    Task PairAsync(Guid doctorUserId, Guid patientUserId, CancellationToken ct);

    Task<IReadOnlyList<PatientSurveyDto>> GetPatientSurveysAsync(
        Guid doctorUserId, Guid patientUserId, DateOnly from, DateOnly to, CancellationToken ct);

    Task<IReadOnlyList<PatientSurveyWithAnswersDto>> GetPatientSurveysWithAnswersAsync(
        Guid doctorUserId, Guid patientUserId, DateOnly from, DateOnly to, CancellationToken ct);
}