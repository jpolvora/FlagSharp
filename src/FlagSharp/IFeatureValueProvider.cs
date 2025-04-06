namespace FlagSharp
{
    public interface IFeatureValueProvider<TValue>
    {
        Task<TValue> ReadAsync(FilterEvaluationContext context);
    }
}
