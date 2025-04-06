using FlagSharp.Patterns;
using System.Linq; // Add this using directive
using System.Threading.Tasks; // Add this using directive

namespace FlagSharp
{

    public class FeatureManager : IFeatureManagerObserver, IDisposable
    {
        private readonly List<IObservableFeatureStore> _featureStores;
        private readonly List<IFeatureFilter> _featureFilters;
        private readonly Dictionary<string, FeatureDefinition> _features = new Dictionary<string, FeatureDefinition>();
        private readonly Dictionary<string, IDisposable> _disposables;
        private bool _isInitialized = false;

        public IObservableFeatureStore ObsersableReference => throw new NotImplementedException();

        public FeatureManager(IEnumerable<IObservableFeatureStore> featureStores, IEnumerable<IFeatureFilter> featureFilters)
        {
            _featureStores = featureStores.ToList();
            _featureFilters = featureFilters.ToList();
            _disposables = new Dictionary<string, IDisposable>();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("FeatureManager is already initialized.");
            }

            _isInitialized = true;

            foreach (var store in _featureStores)
            {
                await store.InitializeAsync();
                await LoadFeaturesFromStore(store);
            }
        }

        private async Task LoadFeaturesFromStore(IObservableFeatureStore store)
        {
            _features.Where(f => f.Value.StoreName == store.StoreName).ToList().ForEach(f => _features.Remove(f.Key));

            var features = await store.GetFeaturesAsync();

            foreach (var feature in features)
            {
                _features[feature.Name] = feature;
            }

            _disposables.Add(store.StoreName, store.Subscribe(this));
        }

        public async Task HandleUpdate(IObservableFeatureStore store)
        {
            if (!_isInitialized)
            {
                return;
            }

            _disposables[store.StoreName]?.Dispose();

            await LoadFeaturesFromStore(store);
        }

        public async Task<bool> IsFeatureEnabledAsync(string featureName, IDictionary<string, object>? state = default)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("FeatureManager is not initialized. Call InitializeAsync() before using it.");
            }

            if (!_features.TryGetValue(featureName, out var feature))
            {
                return false;
            }

            if (!feature.IsEnabled)
            {
                return false;
            }

            IEnumerable<IFeatureFilter> pipelineFilters = _featureFilters.Where(f => f.IsGlobal || feature.RequiredFilterNames.Contains(f.Name));
            var pipeline = new FeatureManagerPipeline(new PipelineContext());
            pipeline.Use(pipelineFilters).Use<ExampleLoggerFilterMiddleware>();
            var filterContext = new FilterEvaluationContext(feature, state);
            bool result = await pipeline.ExecuteAsync(filterContext);

            return result;
        }

        public Task<IEnumerable<FeatureDefinition>> GetFeaturesAsync()
        {
            return Task.FromResult(_features.Values.AsEnumerable());
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables.Values)
            {
                disposable.Dispose();
            }
        }
    }
}
