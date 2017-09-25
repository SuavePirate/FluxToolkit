using System;
using Fluxly.Forms.Startup;
using Microsoft.Practices.ServiceLocation;

namespace Fluxly.Forms.ViewModels
{
    /// <summary>
    /// Helper class for auto binding view models and kicking off the IoC container
    /// </summary>
    public class ViewModelLocator
    {
        public TodoListViewModel TodoList
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TodoListViewModel>();
            }
        }
        public EditTodoViewModel EditTodo
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditTodoViewModel>();
            }
        }
        public CreateTodoViewModel CreateTodo
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CreateTodoViewModel>();
            }
        }
        public ViewModelLocator()
        {
            var iocConfig = new IoCConfig();
            iocConfig.RegisterStores();
            iocConfig.RegisterActions();
            iocConfig.RegisterViewModels();
        }
    }
}
