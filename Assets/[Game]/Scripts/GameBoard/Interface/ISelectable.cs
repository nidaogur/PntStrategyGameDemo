namespace _Game_.Scripts.GameBoard.Interface
{
    public interface ISelectable
    {
        public bool IsSelect { get; set; }
        public virtual void Selected() {}
    }
}