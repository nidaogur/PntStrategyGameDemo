namespace _Game_.Scripts.GameBoard.Interface
{
    public interface IAttack
    {
        float Damage { get; set; }

        void AttackAnimation();
    }
}