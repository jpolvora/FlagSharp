using FlagSharp.Patterns;

namespace FlagSharp
{
    public class ExampleLoggerFilterMiddleware : IFeatureFilter, IMiddleware<FilterEvaluationContext>
    {
        public string Name => nameof(ExampleLoggerFilterMiddleware);

        public bool IsGlobal { get => true; }

        public async Task<bool> EvaluateAsync(FilterEvaluationContext context, Func<IPipelineContext?, Task<bool>> next, CancellationToken? cancellationToken = default)
        {
            // Perform some logic before the next middleware
            Console.WriteLine("Before next middleware");

            // Check for cancellation
            if (cancellationToken?.IsCancellationRequested == true)
            {
                Console.WriteLine("Operation canceled");
                return false;
            }

            // Call the next middleware in the pipeline
            var result = await next(default);

            // Perform some logic after the next middleware
            Console.WriteLine("After next middleware");

            return result;

        }
    }
}
