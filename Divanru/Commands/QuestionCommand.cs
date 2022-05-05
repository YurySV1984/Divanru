using Divanru.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Divanru.Commands
{
    internal class QuestionCommand: Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter) => MessageBox.Show("This application allows to parse DIVAN.RU and choose favorite items");
    }
}
