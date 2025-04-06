namespace FlagSharp.Patterns
{
    public record PipelineContext(bool IsSuccess = false, Exception? Exception = null, object? Data = default) : IPipelineContext
    {
        public IPipelineContext? Previous { get; set; }
    }
}
