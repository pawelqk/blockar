using System;

namespace Utils.StateMachine
{
    public class UnexpectedStateCallException : Exception
    {
        public UnexpectedStateCallException()
        {
        }

        public UnexpectedStateCallException(string message)
            : base(message)
        {
        }

        public UnexpectedStateCallException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public abstract class GenericState<ConcreteContext>
        where ConcreteContext : IStateContext
    { 
        protected ConcreteContext context;

        protected GenericState()
        {
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        protected void SetContext(ConcreteContext context)
        {
            this.context = context;
        }

        public static ConcreteState InitState<ConcreteState>(ConcreteContext context)
            where ConcreteState : GenericState<ConcreteContext>, new()
        {
            var state = new ConcreteState();
            state.SetContext(context);
            context.SetState(state);
            state.OnEnter();
            return state;
        }

        protected void Change<NewState>()
            where NewState : GenericState<ConcreteContext>, new()
        {
            this.OnExit();
            InitState<NewState>(this.context);
        }
    }
}
