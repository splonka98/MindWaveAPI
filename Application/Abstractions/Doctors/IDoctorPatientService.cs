using Application.Contracts.Doctors;

namespace Application.Abstractions.Doctors;

public interface IDoctorPatientService
{
    Task PairAsync(Guid doctorUserId, Guid patientUserId, CancellationToken ct);
    Task<IReadOnlyList<PatientSurveyDto>> GetPatientSurveysAsync(Guid doctorUserId, Guid patientUserId, DateOnly from, DateOnly to, CancellationToken ct);
}