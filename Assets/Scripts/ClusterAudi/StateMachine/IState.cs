namespace ClusterAudi
{
    public interface IState
    {
        public void StateOnEnter();
        public void StateOnExit();
        public void StateOnUpdate();
    }
}
