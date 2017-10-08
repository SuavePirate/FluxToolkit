using System;
namespace FluxToolkit.Definitions
{
    public interface IStore
    {
        event EventHandler<StoreEventArgs> OnEmitted;
    }
    /// <summary>
    /// Definition contract for a Flux Store.
    /// </summary>
    public interface IStore<TData> : IStore
    {
        TData Data { get; set; }
    }
}
