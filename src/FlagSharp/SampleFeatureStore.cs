using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace FlagSharp
{
    public class SampleFeatureStore : IObservableFeatureStore
    {
        private readonly IMemoryCache memoryCache;
        private readonly List<IFeatureManagerObserver> observers = new List<IFeatureManagerObserver>();
        private readonly List<FeatureDefinition> _loadedFeatures;
        private bool _initialized = false;
        private bool _isReady = false;

        public SampleFeatureStore(IMemoryCache memoryCache, IEnumerable<FeatureDefinition>? features = null)
        {
            this.memoryCache = memoryCache;
            _loadedFeatures = features is not null ? [.. features] : [];            
        }

        public string StoreName => nameof(SampleFeatureStore);

        public bool IsReady => _initialized && _isReady;

        public Task InitializeAsync()
        {
            if (_initialized) return Task.CompletedTask;
            _initialized = true;

            return ReloadStore();
        }

        public Task ReloadStore()
        {
            _isReady = false;

            memoryCache.Remove(StoreName);

            var options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };

            using (ICacheEntry entry = memoryCache.CreateEntry(StoreName))
            {
                entry.SetOptions(options)
                     .SetValue(_loadedFeatures)
                     .RegisterPostEvictionCallback(PostEvictionCallback);
            }

            _isReady = true;

            return Task.CompletedTask;
        }

        private void PostEvictionCallback(object key, object? value, EvictionReason reason, object? state)
        {
            Debug.WriteIf(condition: value != null, message: $"PostEvictionCallback: {key} was evicted due to {reason}");

            if (!_isReady) return;

            Task.Run(() => Update());
        }

        public Task<IEnumerable<FeatureDefinition>> GetFeaturesAsync()
        {
            if (!_initialized) throw new InvalidOperationException("FeatureStore is not initialized. Call InitializeAsync() before using this method.");

            if (memoryCache.TryGetValue<List<FeatureDefinition>>(StoreName, out var cachedItems) && cachedItems != null)
            {
                return Task.FromResult<IEnumerable<FeatureDefinition>>(cachedItems);
            }

            return Task.FromResult(Enumerable.Empty<FeatureDefinition>());
        }

        public void Dispose() => observers.Clear();

        public async Task Update()
        {
            await Task.WhenAll(observers.Select(observer => observer.HandleUpdate(this)));
        }

        public IDisposable Subscribe(IFeatureManagerObserver observer)
        {
            observers.Add(observer);
            var subscription = new ObserverSubscritionFeatureStore(this as IObservableFeatureStore, observer as IFeatureManagerObserver);

            return subscription;
        }

        public void Unsubscribe(IFeatureManagerObserver observer)
        {
            observers.Remove(observer);
        }
    }
}
