namespace FlagSharp.Patterns
{
    public class ObserverSubscrition<TObservable, TObserver> : IDisposable where TObservable : IObservableOf<TObserver, TObservable>
    {
        public TObservable Observable { get; }

        public TObserver Observer { get; }

        public ObserverSubscrition(TObservable observable, TObserver observer)
        {
            Observable = observable;
            Observer = observer;
        }     
        public void Dispose()
        {
            Observable.Unsubscribe(Observer);
        }
    }
}