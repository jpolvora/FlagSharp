using FlagSharp.Patterns;

namespace FlagSharp
{

    public interface IObservableFeatureStore : IFeatureStore, IObservableOf<IFeatureManagerObserver, IObservableFeatureStore>
    {

    }

}