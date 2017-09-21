using System;
using System.ComponentModel;

namespace FluxToolkit.Definitions
{
    /// <summary>
    /// Definition of a view model used as part of the modified Flux process.
    /// This is an optional step within the Flux design pattern to adjust for the addition of MVVM
    /// as a sub layer for databinding to the UI.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
    }
}
