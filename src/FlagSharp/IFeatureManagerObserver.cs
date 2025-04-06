using FlagSharp.Patterns;

namespace FlagSharp
{
    public interface IFeatureManagerObserver : IFeatureManager, IObserverOf<IObservableFeatureStore, IFeatureManagerObserver>
    {

    }
}