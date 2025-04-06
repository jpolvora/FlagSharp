namespace FlagSharp
{
    public interface IPipelineContext
    {
        bool IsSuccess => Exception != null;
        Exception? Exception { get; }

        object? Data { get; }

        IPipelineContext? Previous { get; set; }

    }
}
