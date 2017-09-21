using System;
namespace FluxToolkit.Definitions
{
    /// <summary>
    /// Definition contract for a Flux Store.
    /// </summary>
    public interface IStore<TData>
    {
        event EventHandler<StoreEventArgs> OnEmitted;
        TData Data { get; set; }
    }
}
