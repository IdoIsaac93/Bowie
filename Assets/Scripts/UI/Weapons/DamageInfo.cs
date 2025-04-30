public struct DamageInfo
{
    public float Amount;
    public bool IsCritical;
    public bool IsWeakspot;
    public bool IsShieldHit;
    public ArrowheadType DamageType;

    public DamageInfo(float amount, bool isCritical, bool isWeakspot, bool isShieldHit, ArrowheadType damageType)
    {
        Amount = amount;
        IsCritical = isCritical;
        IsWeakspot = isWeakspot;
        IsShieldHit = isShieldHit;
        DamageType = damageType;
    }
}