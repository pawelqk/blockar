namespace Utils.StateMachine
{
    public interface IStateContext
    {
        void SetState<ConcreteContext>(GenericState<ConcreteContext> state)
            where ConcreteContext : IStateContext;
    }
}
