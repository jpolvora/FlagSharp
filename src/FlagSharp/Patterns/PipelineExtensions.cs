using FlagSharp;

namespace FlagSharp.Patterns
{
    public static class PipelineExtensions
    {
        public static Pipeline<TContext> Use<TContext>(this Pipeline<TContext> pipeline, IMiddleware<TContext> middleware)
        {
            pipeline.Use(middleware);
            return pipeline;
        }
        public static Pipeline<TContext> Use<TContext>(this Pipeline<TContext> pipeline, IEnumerable<IMiddleware<TContext>> middlewares)
        {
            foreach (var middleware in middlewares)
            {
                pipeline.Use(middleware);
            }

            return pipeline;
        }

        public static Pipeline<FilterEvaluationContext> Use<TMiddleware>(this Pipeline<FilterEvaluationContext> pipeline) where TMiddleware : class, IMiddleware<FilterEvaluationContext>, new()
        {
            var middleware = new TMiddleware();
            pipeline.Use(middleware);
            return pipeline;
        }
    }
}
