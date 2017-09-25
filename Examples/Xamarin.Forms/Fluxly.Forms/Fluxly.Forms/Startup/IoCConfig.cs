using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluxly.Forms.Actions;
using Fluxly.Forms.Stores;
using Fluxly.Forms.ViewModels;

namespace Fluxly.Forms.Startup
{
    /// <summary>
    /// Wire up some basic IoC for dependency injection
    /// </summary>
    public class IoCConfig
    {
        public IoCConfig()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        public void RegisterActions()
        {
            SimpleIoc.Default.Register<TodoItemActions>();
        }

        public void RegisterStores()
        {
            SimpleIoc.Default.Register<TodoItemStore>();
        }

        public void RegisterViewModels()
        {
            SimpleIoc.Default.Register<TodoListViewModel>();
            SimpleIoc.Default.Register<EditTodoViewModel>();
            SimpleIoc.Default.Register<CreateTodoViewModel>();
        }
    }
}