namespace _Game_.Scripts.GameBoard.Interface
{
    public interface IDamageable
    {
        float Health { get; set; }
        bool IsDead { get; set; }
        public void TakeDamage(float damage,out float remainingHealth);
        public void Death();
    }
}