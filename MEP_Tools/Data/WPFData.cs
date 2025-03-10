using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MEP_Tools
{
    public class WPFData : INotifyPropertyChanged
    { // Định nghĩa các biến thành viên  

        #region 'Event'
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        private MEP_Tools.AutoCutWall.MainView_ArcNew inputwindow_AN;
        public MEP_Tools.AutoCutWall.MainView_ArcNew Inputwindow_AN
        {
            get
            {
                if (inputwindow_AN == null) inputwindow_AN = new MEP_Tools.AutoCutWall.MainView_ArcNew();
                return inputwindow_AN;
            }
        }
        private CreateSiphon.MainView_CreateSiphon inputWindow_CreateSiphone;
        public CreateSiphon.MainView_CreateSiphon TnputWindow_CreateSiphone
        {
            get
            {
                if (inputWindow_CreateSiphone == null) inputWindow_CreateSiphone = new CreateSiphon.MainView_CreateSiphon();
                return inputWindow_CreateSiphone;
            }
        }

        private FeedBack.MainView_FeedBack inputWindow_FeedBack;
        public FeedBack.MainView_FeedBack InputWindow_FeedBack
        {
            get
            {
                if (inputWindow_FeedBack == null) inputWindow_FeedBack = new FeedBack.MainView_FeedBack();
                return inputWindow_FeedBack;
            }
        }


        private ThoatNuoc.MainView_ThoatNuoc inputWindow_ThoatNuoc;
        public ThoatNuoc.MainView_ThoatNuoc InputWindow_ThoatNuoc
        {
            get
            {
                if (inputWindow_ThoatNuoc == null) inputWindow_ThoatNuoc = new ThoatNuoc.MainView_ThoatNuoc();
                return inputWindow_ThoatNuoc;
            }
        }


        private Hanger.MainView_HangerVertical inputWindow_HangerV;
        public Hanger.MainView_HangerVertical InputWindow_HangerV
        {
            get
            {
                if (inputWindow_HangerV == null) inputWindow_HangerV = new Hanger.MainView_HangerVertical();
                return inputWindow_HangerV;
            }
        }
        private Hanger.MainView_HangerHorizontal inputWindow_HangerH;
        public Hanger.MainView_HangerHorizontal InputWindow_HangerH
        {
            get
            {
                if (inputWindow_HangerH == null) inputWindow_HangerH = new Hanger.MainView_HangerHorizontal();
                return inputWindow_HangerH;
            }
        }
        private Register.MainView_User inputWindow_user;
        public Register.MainView_User InputWindow_user
        {
            get
            {
                if (inputWindow_user == null) inputWindow_user = new Register.MainView_User();
                return inputWindow_user;
            }
        }
        private Register.MainView_PW inputWindow_forgetPass;
        public Register.MainView_PW InputWindow_forgetPass
        {
            get
            {
                if (inputWindow_forgetPass == null) inputWindow_forgetPass = new Register.MainView_PW();
                return inputWindow_forgetPass;
            }
        }

        private Register.Mainview_RegisterNew inputWindow_RegisterNew;
        public Register.Mainview_RegisterNew InputWindow_RegisterNew
        {
            get
            {
                if (inputWindow_RegisterNew == null) inputWindow_RegisterNew = new Register.Mainview_RegisterNew();
                return inputWindow_RegisterNew;
            }
        }

        private HolyUpdown.MainView_HUD inputWindow_HUD;
        public HolyUpdown.MainView_HUD InputWindow_HUD
        {
            get
            {
                if (inputWindow_HUD == null) inputWindow_HUD = new HolyUpdown.MainView_HUD();
                return inputWindow_HUD;
            }
        }

        private MEPUpdown.MainView_MUD inputWindow_MUD;
        public MEPUpdown.MainView_MUD InputWindow_MUD
        {
            get
            {
                if (inputWindow_MUD == null) inputWindow_MUD = new MEPUpdown.MainView_MUD();
                return inputWindow_MUD;
            }
        }
        private SplitPipe.MainView_SP inputWindow_SP;
        public SplitPipe.MainView_SP InputWindow_SP
        {
            get
            {
                if (inputWindow_SP == null) inputWindow_SP = new SplitPipe.MainView_SP();
                return inputWindow_SP;
            }
        }
        private Register.MainView_Register inputWindow_Register;
        public Register.MainView_Register InputWindow_Register
        {
            get
            {
                if (inputWindow_Register == null) inputWindow_Register = new Register.MainView_Register();
                return inputWindow_Register;
            }
        }
        private Contact contactWindow;
        public Contact ContactWindow
        {
            get
            {
                if (contactWindow == null) contactWindow = new Contact();
                return contactWindow;
            }
        }

        private SA.MainView_SA inputwindow_SA;
        public SA.MainView_SA Inputwindow_SA
        {
            get
            {
                if (inputwindow_SA == null) inputwindow_SA = new SA.MainView_SA();
                return inputwindow_SA;
            }
        }

        public class RelayCommand<T> : ICommand
        {
            private readonly Predicate<T> _canExecute;
            private readonly Action<T> _execute;

            public RelayCommand(Predicate<T> canExecute, Action<T> execute)
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");
                _canExecute = canExecute;
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                try
                {
                    return _canExecute == null ? true : _canExecute((T)parameter);
                }
                catch
                {
                    return true;
                }
            }

            public void Execute(object parameter)
            {
                _execute((T)parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
        public class RelayCommand : ICommand
        {
            private Action<object> execute;
            private Func<object, bool> canExecute;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return this.canExecute == null || this.canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                this.execute(parameter);
            }
        }
    }
}