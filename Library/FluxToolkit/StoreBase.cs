using System;
using FluxToolkit.Definitions;

namespace FluxToolkit
{
    public abstract class StoreBase : IStore
    {

		/// <summary>
		/// Event fired when an event is received from the dispatcher and processed
		/// </summary>
		public event EventHandler<StoreEventArgs> OnEmitted;
		public string Error { get; set; }

		/// <summary>
		/// Emits an event received from the dispatcher to anything subscribed to the handler
		/// </summary>
		/// <param name="eventType"></param>
		protected void Emit(string eventType)
		{
			OnEmitted?.Invoke(this, new StoreEventArgs(eventType));
		}

		/// <summary>
		/// Subscribes to an event from the dispatcher
		/// </summary>
		/// <typeparam name="TData">The type of data the dispatcher is sending</typeparam>
		/// <param name="eventType">The name of the event to subscribe to</param>
		protected virtual void Subscribe<TArgs>(string eventType)
		{
			Dispatcher.Subscribe<IActions, TArgs>(this, eventType, (sender, data) => ReceiveEvent(eventType, data));
		}

		/// <summary>
		/// Subscribes to an event from the dispatcher with no data
		/// </summary>
		/// <param name="eventType">The name of the event to subscribe to</param>
		protected virtual void Subscribe(string eventType)
		{
			Dispatcher.Subscribe<IActions>(this, eventType, (sender) => ReceiveEvent(eventType, (object)null));
		}

		/// <summary>
		/// Called when an event is received from the dispatcher.
		/// Override this to handle when an event is received from the dispatcher.
		/// </summary>
		/// <typeparam name="TData">The type of data from the dispatcher</typeparam>
		/// <param name="eventType">The name of the event being fied</param>
		/// <param name="data">The data sent from the dispatcher</param>
		protected virtual void ReceiveEvent<TArgs>(string eventType, TArgs data)
		{
			// when we receive an event, we need to ensure that the event is then emitted to the views that are subscribed to the store.
			Emit(eventType);
		}
    }

    /// <summary>
    /// Base store to handle shared methods
    /// </summary>
    public abstract class StoreBase<TData> : StoreBase, IStore<TData>
    {

        /// <summary>
        /// The primary data in stored
        /// </summary>
        public TData Data { get; set; }
       
    }
}
