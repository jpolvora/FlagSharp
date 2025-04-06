using FlagSharp.Patterns;

namespace FlagSharp
{
    public interface IFeatureFilter : IMiddleware<FilterEvaluationContext>
    {
        string Name { get; }
        public bool IsGlobal { get; }
    }
}
