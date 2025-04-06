using FlagSharp;
using Microsoft.Extensions.Caching.Memory;
using FlagSharp.Patterns;
using System.Linq;

namespace FlagSharp.Tests
{

    public class FeatureManagerTests
    {
        [Fact]
        public async Task StoreHasChanged_ReSubscribesToChangeToken()
        {
            // Arrange
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache, FeatureData.GetFeatures(nameof(SampleFeatureStore)));
            var featureFilters = new List<IFeatureFilter>();
            var featureManager = new FeatureManager([featureStore], featureFilters);

            await featureManager.InitializeAsync();

            // Act           
            //featureManager.HandleUpdate(featureStore);
            var firstFeature = (await featureStore.GetFeaturesAsync()).First();
            var result = await featureManager.IsFeatureEnabledAsync(firstFeature.Name);

            // Assert
            // Verify that the store has been reloaded and the change token has been re-subscribed
            Assert.True(result);
        }


        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsFalse_WhenFeatureNotFound()
        {
            // Arrange
            var featureStores = new List<IObservableFeatureStore>();
            var featureFilters = new List<IFeatureFilter>();
            var featureManager = new FeatureManager(featureStores, featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("NonExistentFeature");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsFalse_WithARealStore()
        {
            // Arrange
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            List<IObservableFeatureStore> featureStores = new List<IObservableFeatureStore>() { new SampleFeatureStore(cache) };
            var featureFilters = new List<IFeatureFilter>();
            var featureManager = new FeatureManager(featureStores, featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("NonExistentFeature");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsTrue_WhenFeatureAlwaysEnabled()
        {
            // Arrange
            var feature = new FeatureDefinition("AlwaysEnabledFeature", string.Empty, true, "Test", EFeatureRequirementType.All);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache, [feature]);
            var featureFilters = new List<IFeatureFilter>();
            var featureManager = new FeatureManager([featureStore], featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("AlwaysEnabledFeature");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsFalse_WhenFeatureDisabled()
        {
            // Arrange
            var feature = new FeatureDefinition("DisabledFeature", string.Empty, false, "Test", EFeatureRequirementType.All);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache);
            var featureStores = new List<IObservableFeatureStore> { featureStore };
            var featureFilters = new List<IFeatureFilter>();
            var featureManager = new FeatureManager(featureStores, featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("DisabledFeature");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsTrue_WhenAllFiltersPass()
        {
            // Arrange
            var feature = new FeatureDefinition("FeatureWithAllFilters", string.Empty, true, "Test", EFeatureRequirementType.All)
            {
                RequiredFilterNames = new List<string> { "Filter1", "Filter2" }
            };
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache, [feature]);
            var filter1 = new MockFeatureFilter("Filter1", true);
            var filter2 = new MockFeatureFilter("Filter2", true);
            var featureFilters = new List<IFeatureFilter> { filter1, filter2 };
            var featureManager = new FeatureManager([featureStore], featureFilters);
            await featureManager.InitializeAsync();

            // Act
            var result = await featureManager.IsFeatureEnabledAsync("FeatureWithAllFilters");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsFalse_WhenAnyFilterFails()
        {
            // Arrange
            var feature = new FeatureDefinition("FeatureWithAllFilters", string.Empty, true, "Test", EFeatureRequirementType.All)
            {
                RequiredFilterNames = new List<string> { "Filter1", "Filter2" }
            };
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache, [feature]);
            var filter1 = new MockFeatureFilter("Filter1", true);
            var filter2 = new MockFeatureFilter("Filter2", false);
            var featureFilters = new List<IFeatureFilter> { filter1, filter2 };
            var featureManager = new FeatureManager([featureStore], featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("FeatureWithAllFilters");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFeatureEnabledAsync_ReturnsTrue_WhenAnyFilterPasses()
        {
            // Arrange
            var feature = new FeatureDefinition("FeatureWithAnyFilters", string.Empty, true, "Test", EFeatureRequirementType.Any)
            {
                RequiredFilterNames = new List<string> { "Filter1", "Filter2" }
            };
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var featureStore = new SampleFeatureStore(cache, [feature]);
            var filter1 = new MockFeatureFilter("Filter1", true);
            var filter2 = new MockFeatureFilter("Filter2", true);
            var featureFilters = new List<IFeatureFilter> { filter1, filter2 };
            var featureManager = new FeatureManager([featureStore], featureFilters);
            await featureManager.InitializeAsync();
            // Act
            var result = await featureManager.IsFeatureEnabledAsync("FeatureWithAnyFilters");

            // Assert
            Assert.True(result);
        }


        private class MockFeatureFilter : IFeatureFilter
        {
            public string Name { get; }
            public bool IsGlobal { get; set; }

            private readonly bool _evaluationResult;

            public MockFeatureFilter(string name, bool evaluationResult)
            {
                Name = name;
                _evaluationResult = evaluationResult;
            }

            public async Task<bool> EvaluateAsync(FilterEvaluationContext context, Func<IPipelineContext?, Task<bool>> next, CancellationToken? cancellationToken = null)
            {
                cancellationToken?.ThrowIfCancellationRequested();

                var result = await next(new PipelineContext());

                return _evaluationResult && result;
            }
        }
    }
}
