using FlagSharp;
using Microsoft.Extensions.Caching.Memory;

namespace FlagSharp.Tests
{
    public class FeatureStoreTests
    {
        [Fact]
        public async Task LoadFeaturesAsync_ReturnsEmptyList_WhenNoFeatures()
        {
            // Arrange
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            //FeatureData.GetFeatures(nameof(SampleFeatureStore))
            SampleFeatureStore featureStore = new(cache);

            featureStore.Dispose();

            // Act
            await featureStore.InitializeAsync();
            var features = await featureStore.GetFeaturesAsync();
            // Assert
            // Assert.True(features.Count() == FeatureData.GetFeatures(featureStore.StoreName).Count);
            Assert.True(features.Count() == 0);
        }
    }
}
