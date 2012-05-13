using System;

namespace Starbucks.Wcf
{
    public static class ServiceLocator
    {
        public static Action<Object> Dispatch { get; private set; }

        public static void RegisterDispatcher<TUseCase>(Action<TUseCase> dispatch)
        {
            Dispatch = message => dispatch((TUseCase) message);
        }
    }
}