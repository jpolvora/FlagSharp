using System.Diagnostics;
using System.Threading;

namespace FlagSharp.Patterns
{

    public abstract class Pipeline<TContext>
    {
        private readonly List<IMiddleware<TContext>> _middlewares = new();

        public Pipeline(IPipelineContext context)
        {
            Context = context;
        }

        public void Use(IMiddleware<TContext> middleware)
        {
            _middlewares.Add(middleware);
        }

        public int Count => _middlewares.Count;

        public IPipelineContext Context { get; }

        public virtual Task<bool> ExecuteAsync(TContext context, CancellationToken? cancellationToken = default)
        {
            cancellationToken = cancellationToken is null ? new CancellationTokenSource().Token : cancellationToken;

            return ExecuteMiddleware(0, context, cancellationToken);
        }

        private Func<IPipelineContext?, Task<bool>> GetNext(int index, TContext context, CancellationToken? cancellationToken)
             => (ctx) => ExecuteMiddleware(index + 1, context, cancellationToken, ctx);

        protected virtual async Task<bool> ExecuteMiddleware(int index, TContext context, CancellationToken? cancellationToken, IPipelineContext? ctx = null)
        {
            if (ctx is { Exception: not null } && ctx.IsSuccess == false)
            {
                // Handle the exception or log it
                Debug.WriteLine($"Exception in middleware at index {index}: {ctx.Exception.Message}");
                return false;
            }
            try
            {
                cancellationToken?.ThrowIfCancellationRequested();

                if (index < _middlewares.Count)
                {
                    var filter = _middlewares[index];
                    var next = GetNext(index, context, cancellationToken);

                    return await filter.EvaluateAsync(context, next, cancellationToken);
                }
            }
            catch (Exception operationCanceledEx)
            {
                ctx = ctx ?? new PipelineContext(false, operationCanceledEx);
            }
            return true; // End of pipeline
        }
    }
}

