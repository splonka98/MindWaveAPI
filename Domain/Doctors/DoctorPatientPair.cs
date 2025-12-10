namespace Domain.Doctors;

public sealed class DoctorPatientPair
{
    public Guid Id { get; private set; }
    public Guid DoctorUserId { get; private set; }
    public Guid PatientUserId { get; private set; }

    private DoctorPatientPair() { }

    public static DoctorPatientPair Create(Guid id, Guid doctorUserId, Guid patientUserId)
    {
        return new DoctorPatientPair
        {
            Id = id,
            DoctorUserId = doctorUserId,
            PatientUserId = patientUserId
        };
    }
}