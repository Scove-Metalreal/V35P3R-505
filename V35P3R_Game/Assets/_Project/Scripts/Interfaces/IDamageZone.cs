namespace _Project.Scripts.Interfaces
{
    public interface IDamageZone
    {
        float GetDamageAmount();     // Cho va chạm 1 lần (Mìn nổ)
        float GetDamagePerSecond();  // Cho va chạm liên tục (Lava/Độc)
    }
}