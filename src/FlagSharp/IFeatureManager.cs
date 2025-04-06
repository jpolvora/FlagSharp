namespace FlagSharp
{
    public interface IFeatureManager
    {
        Task InitializeAsync();
        Task<IEnumerable<FeatureDefinition>> GetFeaturesAsync();
        Task<bool> IsFeatureEnabledAsync(string featureName, IDictionary<string, object>? state = default);
    }
}