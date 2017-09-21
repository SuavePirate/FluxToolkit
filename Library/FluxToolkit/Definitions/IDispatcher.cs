using System;
namespace FluxToolkit.Definitions
{
    /// <summary>
    /// Definition for a dispatcher to enable subscribing, unsubscribing, and sending of events  
    /// - inspired by the Xamarin.Forms Messaging Center using Reflection
    /// </summary>
    public interface IDispatcher
    {
        void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class;

        void Send<TSender>(TSender sender, string message) where TSender : class;

        void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class;

        void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class;

        void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class;

        void Unsubscribe<TSender>(object subscriber, string message) where TSender : class;
    }

}
