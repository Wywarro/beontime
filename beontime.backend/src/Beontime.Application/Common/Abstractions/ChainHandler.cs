namespace Beontime.Application.Common.Abstractions
{
    public abstract class ChainHandler : IHandler
    {
        private IHandler nextHandler = null!;

        public IHandler SetNext(IHandler handler)
        {
            nextHandler = handler;
            return handler;
        }

        public virtual object? Handle()
        {
            if (nextHandler is not null)
            {
                return nextHandler.Handle();
            }

            return null;
        }
    }
}
