namespace FlagSharp
{
    public interface IFeatureValueWriter<TContext, TValue>
    {
        Task<bool> WriteAsync(string featureName, IDictionary<string, object> parameters, TValue value);
    }
}
