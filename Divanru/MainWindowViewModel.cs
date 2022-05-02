using Divanru.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Divanru
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title
        private string _title = "Divanru parser";
        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get { return _title; }
            //set
            //{
            //    if (_title == value) return;
            //    _title = value;
            //    OnPropertyChanged();
            //    Set(ref _title, value);
            //}
            set => Set(ref _title, value);
        }
        #endregion
        #region status
        private string _status = "Ready";
        public string Status
        { 
            get { return _status; }
            set => Set(ref _status, value);
        }
        #endregion
        #region commands

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted (object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion
        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
        }
    }
}
