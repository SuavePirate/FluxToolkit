using System;
using Fluxly.Forms.Actions;

namespace Fluxly.Forms.ViewModels
{
    public class CreateTodoViewModel : BasePageViewModel
    {
        private readonly TodoItemActions _actions;
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                Set(ref _text, value);
            }
        }
        public CreateTodoViewModel(TodoItemActions actions)
        {
            _actions = actions;
        }
    }
}
