namespace Domain.Pairing;

public sealed class PairingToken
{
    public Guid Id { get; private set; }
    public Guid PatientUserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool Redeemed { get; private set; }

    private PairingToken() { }

    public static PairingToken Issue(Guid id, Guid patientUserId, string code, DateTime expiresAtUtc)
    {
        return new PairingToken
        {
            Id = id,
            PatientUserId = patientUserId,
            Code = code,
            ExpiresAtUtc = expiresAtUtc,
            Redeemed = false
        };
    }

    public bool CanRedeem(DateTime nowUtc) => !Redeemed && nowUtc <= ExpiresAtUtc;

    public void MarkRedeemed()
    {
        if (Redeemed) return;
        Redeemed = true;
    }
}