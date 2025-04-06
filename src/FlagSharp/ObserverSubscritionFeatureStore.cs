using FlagSharp.Patterns;

namespace FlagSharp
{
    public class ObserverSubscritionFeatureStore : ObserverSubscrition<IObservableFeatureStore, IFeatureManagerObserver>
    {
        public ObserverSubscritionFeatureStore(IObservableFeatureStore observable, IFeatureManagerObserver observer)
            : base(observable, observer)
        {
        }
    }
}