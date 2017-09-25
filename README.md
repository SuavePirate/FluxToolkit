# FluxToolkit
A super simple library to enable the implementation of Flux in .NET Applications such as Xamarin, UWP, WPF, and more. It contains some base level Flux components to help you get started with your implementation faster.

# What is Flux?

Flux is a design pattern created by Facebook with the purpose of creating robust data-driven UI components and handles the flow of data between components and outward to services.

## Components
Flux consists of 4 major components - Stores, Actions, Components, and the Dispatcher

### Stores

Stores are responsible for containing and managing data for a single domain or data type. Stores listen to the dispatcher for certain events and use the data from the dispatcher to update their data, handle errors, and then pass that update down to the components that are subscribed to the store.

### Actions

Actions are responsible for piping events through the dispatcher. Actions are invoked from Components or from background processes. They can also handle some small business logic such as data mapping or talking to external services.

### Components

Components are the UI and UI logic layers. They are responsible for displaying views to the users and for handling user events. They invoke Actions and subsribe to Stores to handle updates to the data.

### Dispatcher

A single dispatcher is responsible for the Pub/Sub mechanism of events invoked from Actions. Stores subscribe to events by name through the Dispatcher.

## How does it work with MVVM and data binding?

ViewModels can be considered part of the Component layer but are separated from the actual UI/Views. This means that the ViewModels are responsible for invoking Actions, and subscribing to Stores. The Views themselves are only responsible for showing the UI and communicating to the ViewModel.

# Getting Started

## Install

The FluxToolkit is available on Nuget: https://www.nuget.org/packages/FluxToolkit 
It has no external dependencies and should work with any .NET Standard library or project including Xamarin, Xamarin.Forms, UWP, WPF, and even WinForms. It has not been used for web application development, but it is compatible with ASP.NET projects.

Install the nuget package with the nuget package manager or via the Package Manager command line:

```
Install-Package FluxToolkit
```

## Create your Stores

Use the `StoreBase` class from the FluxToolkit to implement your unique stores for your different data types. It contains a generic `Data` field based on the `TData` type you pass into the definition. Now you don't have to worry about communicating to the dispatcher for pub/sub - simply call the base methods for `Subsribe` and `Unsubscribe`.

Ensure that you are not using multiple instances of your `Stores`, but rather should be using either a `Singleton` or Inversion of Control with Dependency Injection to pass the implementation of your `Store` to the `Components` that require it through the constructor. Constantly creating new `Stores` can cause memory leaks due to the event subscriptions.

Here's an example store implementation:

``` csharp

    /// <summary>
    /// Event store for holding and managing todo items
    /// </summary>
    public class TodoStore : StoreBase<ObservableCollection<Todo>>
    {
        /// <summary>
        /// Creates a new store and handles subscriptions to the dispatcher
        /// </summary>
        public TodoStore()
        {
            Subscribe<string>(TodoActionTypes.ADD_TODO);
            Subscribe(TodoActionTypes.DELETE_COMPLETED_TODOS);
            Subscribe<string>(TodoActionTypes.DELETE_TODO);
            Subscribe<Todo>(TodoActionTypes.EDIT_TODO);
            Subscribe(TodoActionTypes.TOGGLE_ALL_TODOS);
            Subscribe<string>(TodoActionTypes.TOGGLE_TODO);

            Data = new ObservableCollection<Todo>();
        }

        /// <summary>
        /// Processes an event from the dispatcher before emitting it.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        protected override void ReceiveEvent<TData>(string eventType, TData data)
        {
            try
            {
                Error = null;
                switch (eventType)
                {
                    case TodoActionTypes.ADD_TODO:
                        Data.Add(new Todo
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = data as string,
                            IsComplete = false
                        });
                        break;
                    case TodoActionTypes.DELETE_COMPLETED_TODOS:
                        var itemsToRemove = Data.Where(t => t.IsComplete);
                        foreach(var item in itemsToRemove.ToList())
                        {
                            Data.Remove(item);
                        }
                        break;
                    case TodoActionTypes.DELETE_TODO:
                        var itemToRemove = Data.FirstOrDefault(t => t.Id == data as string);
                        if (itemToRemove != null)
                            Data.Remove(itemToRemove);
                        break;
                    case TodoActionTypes.EDIT_TODO:
                        var itemToEdit = Data.FirstOrDefault(t => t.Id == (data as Todo).Id);
                        if (itemToEdit != null)
                            itemToEdit.Text = (data as Todo).Text;
                        break;
                    case TodoActionTypes.TOGGLE_ALL_TODOS:
                        var areAllComplete = !Data.Any(t => !t.IsComplete);
                        foreach(var todo in Data)
                        {
                            todo.IsComplete = !areAllComplete;
                        }
                        break;
                    case TodoActionTypes.TOGGLE_TODO:
                        var itemToToggle = Data.First(t => t.Id == (data as string));
                        if (itemToToggle != null)
                            itemToToggle.IsComplete = !itemToToggle.IsComplete;
                        break;

                }
            }
            catch (Exception ex)
            {
                // if something goes wrong, set the error before emitting
                Error = ex.Message;
            }
           
            base.ReceiveEvent(eventType, data);
        }
    }
```

## Create your Actions

Create an `Actions` class for each of your main data types. These actions will call to the `Dispatcher` to fire events and will also need to implement `IActions` in order to properly handle the pub/sub mechanism.

``` csharp
    /// <summary>
    /// Actions to be taken against Todo items
    /// </summary>
    public class TodoActions : IActions
    {
        public void AddTodo(string text)
        {
            Dispatcher.Send<IActions, string>(this, TodoActionTypes.ADD_TODO, text);
        }

        public void DeleteCompletedTodos()
        {
            Dispatcher.Send<IActions>(this, TodoActionTypes.DELETE_COMPLETED_TODOS);
        }

        public void DeleteTodo(string id)
        {
            Dispatcher.Send<IActions, string>(this, TodoActionTypes.DELETE_TODO, id);
        }

        public void EditTodo(string id, string text)
        {
            Dispatcher.Send<IActions, Todo>(this, TodoActionTypes.EDIT_TODO, new Todo
            {
                Id = id,
                Text = text
            });
        }

        public void StartEditingTodo(string id)
        {
            Dispatcher.Send<IActions, string>(this, TodoActionTypes.START_EDITING_TODO, id);
        }

        public void StopEditingTodo()
        {
            Dispatcher.Send<IActions>(this, TodoActionTypes.STOP_EDITING_TODO);
        }

        public void ToggleAllTodos()
        {
            Dispatcher.Send<IActions>(this, TodoActionTypes.TOGGLE_ALL_TODOS);
        }

        public void ToggleTodo(string id)
        {
            Dispatcher.Send<IActions, string>(this, TodoActionTypes.TOGGLE_TODO, id);
        }
        
    }
```

## Define your ActionTypes

For each of your data types, you'll need to define some `ActionTypes` which translate to the `id` or `name` of the events your `Actions` are invoking through the `Dispatcher`. 

``` csharp
    /// <summary>
    /// Different types of actions that can be completed within the context of Todo items
    /// </summary>
    public class TodoActionTypes
    {
        public const string ADD_TODO = "add_todo";
        public const string DELETE_COMPLETED_TODOS = "delete_completed_todos";
        public const string DELETE_TODO = "delete_todo";
        public const string EDIT_TODO = "edit_todo";
        public const string START_EDITING_TODO = "start_editing_todo";
        public const string STOP_EDITING_TODO = "stop_editing_todo";
        public const string TOGGLE_ALL_TODOS = "toggle_all_todos";
        public const string TOGGLE_TODO = "toggle_todo";
        public const string UPDATE_DRAFT = "update_draft";
    }
```

## Wire up your Components (with MVVM or without)

Have your components subscribe to the `Stores` that are appropriate for the data need, and invoke the `Actions` they need. This is a great place to place inject your `Stores` and `Actions` into the constructor of your `Components` whether it is through a `ViewModel` or an `Activity`, `ViewController`, or `Xamarin.Forms.Page`.

``` csharp
    public class TodoListPageViewModel : BasePageViewModel
    {
        private readonly TodoStore _todoStore;
        private readonly TodoActions _todoActions;
        private ObservableCollection<Todo> _items;
        private ICommand _createCommand;
        private ICommand _toggleCommand;
        private ICommand _toggleAllCommand;
        private ICommand _deleteCommand;
        private ICommand _deleteCompletedCommand;
        private ICommand _editCommand;
        private ICommand _populateCommand;

        public ICommand CreateCommand
        {
            get
            {
                return _createCommand ??
                    (_createCommand = new RelayCommand(async () =>
                    {
                        var result = await UserDialogs.Instance.PromptAsync(string.Empty, "New", "Done", "Cancel", "Todo...");
                        if (result.Ok)
                        {
                            _todoActions.AddTodo(result.Text);
                        }
                    }));
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return _editCommand ??
                    (_editCommand = new RelayCommand<Todo>(async (t) =>
                    {
                        var result = await UserDialogs.Instance.PromptAsync(new PromptConfig()
                            .SetText(t.Text)
                            .SetTitle("Edit")
                            .SetOkText("Done")
                            .SetCancelText("Cancel")
                            .SetPlaceholder("Todo..."));
                        if (result.Ok)
                        {
                            _todoActions.EditTodo(t.Id, result.Text);
                        }
                    }));
            }
        }

        public ICommand ToggleCommand
        {
            get
            {
                return _toggleCommand ??
                    (_toggleCommand = new RelayCommand<Todo>((t) =>
                    {
                        _todoActions.ToggleTodo(t.Id);
                    }));
            }
        }

        public ICommand ToggleAllCommand
        {
            get
            {
                return _toggleAllCommand ??
                    (_toggleAllCommand = new RelayCommand(() =>
                    {
                        _todoActions.ToggleAllTodos();
                    }));
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                    (_deleteCommand = new RelayCommand<Todo>((t) =>
                    {
                        _todoActions.DeleteTodo(t.Id);
                    }));
            }
        }

        public ICommand DeleteCompletedCommand
        {
            get
            {
                return _deleteCompletedCommand ??
                    (_deleteCompletedCommand = new RelayCommand(() =>
                    {
                        _todoActions.DeleteCompletedTodos();
                    }));
            }
        }

        public ICommand PopulateCommand
        {
            get
            {
                return _populateCommand ??
                    (_populateCommand = new RelayCommand(() =>
                    {
                        for(var i = 1; i < 20; i++)
                        {
                            _todoActions.AddTodo($"New Item {i}");
                            Task.Delay(200);
                        }
                    }));
            }
        }

        public ObservableCollection<Todo> Items
        {
            get
            {
                return _todoStore.Data;
            }
        }

        public TodoListPageViewModel(TodoStore todoStore, TodoActions todoActions)
        {
            _todoStore = todoStore;
            _todoActions = todoActions;
            _todoStore.OnEmitted += TodoStore_OnEmitted;
        }

        /// <summary>
        /// Processes events from the todo store and updates any UI that isn't handled automatically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodoStore_OnEmitted(object sender, StoreEventArgs e)
        {
            switch (e.EventType)
            {
                case TodoActionTypes.ADD_TODO:
                    if(_todoStore.Error == null)
                    {
                        UserDialogs.Instance.Toast("Item added");
                    }
                    break;
                case TodoActionTypes.DELETE_COMPLETED_TODOS:
                    if (_todoStore.Error == null)
                    {
                        UserDialogs.Instance.Toast("Items deleted");
                    }
                    break;
                case TodoActionTypes.DELETE_TODO:
                    if (_todoStore.Error == null)
                    {
                        UserDialogs.Instance.Toast("Item deleted");
                    }
                    break;
                case TodoActionTypes.TOGGLE_ALL_TODOS:
                    if (_todoStore.Error == null)
                    {
                        UserDialogs.Instance.Toast("Items toggled");
                    }
                    break;
                case TodoActionTypes.TOGGLE_TODO:
                    if (_todoStore.Error == null)
                    {
                        UserDialogs.Instance.Toast("Item toggled");
                    }
                    break;
            }
            if(_todoStore.Error != null)
            {
                UserDialogs.Instance.ShowError(_todoStore.Error);
            }
        }
    }
```
# Contributing

Want to add additional examples or more tooling to help people develop their apps with Flux? Fork this repository and create a pull request!

# Additional Resources

- Xamarin Univeristy Guest Lecture: https://university.xamarin.com/guestlectures/architecting-your-app-with-xamarin-facebook-flux
- Xamarin.Flux Template Repository: https://github.com/SuavePirate/Xamarin.Flux



