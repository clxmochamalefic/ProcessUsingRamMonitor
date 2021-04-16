using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessUsingRamMonitor.UserControls.Models
{
    public class MonitoringAreaModel : BindableBase
    {
        private int _pid;
        public int Pid
        {
            get { return _pid; }
            set { SetProperty(ref _pid, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Process MonitoringProcess { get; set; }
        
        private ObservableCollection<MemoryDataModel> _ramMonitorDatas = new();
        public ObservableCollection<MemoryDataModel> RamMonitorDatas
        {
            get { return _ramMonitorDatas; }
            set { SetProperty(ref _ramMonitorDatas, value); }
        }
    }
}
