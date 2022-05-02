using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru.Commands.Base
{
    internal class LambdaCommand : Command
    {
        private readonly Action<Object> _execute;
        private readonly Func<Object, bool> _canExecute;
        public LambdaCommand(Action<Object> Execute, Func<Object, bool> CanExecute = null)
        {
            _execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _canExecute = CanExecute;
        }
        public override bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => _execute(parameter);
    }
}
