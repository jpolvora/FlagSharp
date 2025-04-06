namespace FlagSharp.Patterns
{
    public interface IObserverOf<TObservable, TObserver> where TObservable : IObservableOf<TObserver, TObservable>
    {
        public TObservable ObsersableReference { get; }

        Task HandleUpdate(TObservable observable);
    }
}
