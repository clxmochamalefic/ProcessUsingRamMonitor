using Prism.Mvvm;

namespace ProcessUsingRamMonitor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "ProcessUsingRamMonitor";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
