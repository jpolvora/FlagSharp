using FlagSharp.Patterns;

namespace FlagSharp
{
    public class AlwaysRunMiddleware : IFeatureFilter, IMiddleware<FilterEvaluationContext>
    {
        public string Name => nameof(AlwaysRunMiddleware);

        public bool IsGlobal { get => true; }

        public async Task<bool> EvaluateAsync(FilterEvaluationContext context, Func<IPipelineContext?, Task<bool>> next, CancellationToken? cancellationToken = default)
        {
            try
            {
                cancellationToken?.ThrowIfCancellationRequested();

                var feature = context.FeatureDefinition;
                var parameters = context.State as IDictionary<string, object>;
                if (parameters != null)
                {
                    if (parameters.ContainsKey("ForceResult"))
                    {
                        return bool.TryParse(parameters["ForceResult"].ToString(), out _);
                    }
                }

                if (feature.RequirementType == EFeatureRequirementType.All)
                {
                    return await next(default);
                }
                else if (feature.RequirementType == EFeatureRequirementType.Any)
                {

                    return false;
                }
            }
            catch (Exception ex)
            {
                await next(new PipelineContext(false, ex));
            }

            return false;
        }
    }
}
