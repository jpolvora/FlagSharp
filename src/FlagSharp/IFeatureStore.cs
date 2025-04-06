namespace FlagSharp
{
    public interface IFeatureStore : IDisposable
    {
        string StoreName { get; }
        Task InitializeAsync();
        bool IsReady { get; }
        Task<IEnumerable<FeatureDefinition>> GetFeaturesAsync();
    }
}