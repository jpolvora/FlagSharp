using FlagSharp.Patterns;

namespace FlagSharp
{
    public class FeatureManagerPipeline : Pipeline<FilterEvaluationContext>
    {
        public FeatureManagerPipeline(IPipelineContext context) : base(context)
        {
        }
    }
}

