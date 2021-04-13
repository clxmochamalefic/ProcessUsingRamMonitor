using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ProcessUsingRamMonitor.UserControls.Models
{
    public record ProcessDetailModel
    {
        public int Pid { get; set; }
        public string Name { get; set; }
        public BitmapSource Image { get; set; }
        public bool IsCheck { get; set; }
    }

    public class ProcessAreaModel : BindableBase
    {
        private string _findProcessName = "";
        public string FindProcessName
        {
            get { return _findProcessName; }
            set { SetProperty(ref _findProcessName, value); }
        }

        private ObservableCollection<ProcessDetailModel> _processDetails = new();
        public ObservableCollection<ProcessDetailModel> ProcessDetails
        {
            get { return _processDetails; }
            set { SetProperty(ref _processDetails, value); }
        }

        private ProcessDetailModel _selectedProcess = new();
        public ProcessDetailModel SelectedProcess
        {
            get { return _selectedProcess; }
            set { SetProperty(ref _selectedProcess, value); }
        }

        private string _recordingStatus;
        public string RecordingStatus
        {
            get { return _recordingStatus; }
            set { SetProperty(ref _recordingStatus, value); }
        }
    }
}
