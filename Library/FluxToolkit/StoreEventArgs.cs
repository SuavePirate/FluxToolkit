using System;
namespace FluxToolkit
{
    /// <summary>
    /// Event args for an emitted event from a store
    /// </summary>
    public class StoreEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the event being executed or emitted
        /// </summary>
        /// <value>The type of the event.</value>
        public string EventType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FluxToolkit.StoreEventArgs"/> class with the event type name.
        /// </summary>
        /// <param name="eventType">Event type.</param>
        public StoreEventArgs(string eventType)
        {
            EventType = eventType;
        }
    }

}
