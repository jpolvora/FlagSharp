namespace FlagSharp.Patterns
{
    public interface IMiddleware<TContext>
    {
        Task<bool> EvaluateAsync(TContext context, Func<IPipelineContext?, Task<bool>> next, CancellationToken? cancellationToken = default);
    }
}
