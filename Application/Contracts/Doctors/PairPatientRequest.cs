namespace Application.Contracts.Doctors;

public sealed class PairPatientRequest
{
    public Guid PatientUserId { get; init; }
}