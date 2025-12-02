namespace Domain.Pairing;

public sealed class DoctorPatientLink
{
    public Guid Id { get; private set; }
    public Guid DoctorUserId { get; private set; }
    public Guid PatientUserId { get; private set; }
    public DateTime LinkedAtUtc { get; private set; }

    private DoctorPatientLink() { }

    public static DoctorPatientLink Create(Guid id, Guid doctorUserId, Guid patientUserId, DateTime linkedAtUtc)
    {
        return new DoctorPatientLink
        {
            Id = id,
            DoctorUserId = doctorUserId,
            PatientUserId = patientUserId,
            LinkedAtUtc = linkedAtUtc
        };
    }
}