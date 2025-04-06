namespace FlagSharp.Patterns
{
    public interface IObservableOf<in TObserver, out TObservable> : IDisposable
            where TObservable : IObservableOf<TObserver, TObservable>
    {
        IDisposable Subscribe(TObserver observer);
        void Unsubscribe(TObserver observer);
        Task Update();
    }
}