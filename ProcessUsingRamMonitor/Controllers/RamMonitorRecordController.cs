using ProcessUsingRamMonitor.UserControls.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessUsingRamMonitor.Controllers
{
    public class RamMonitorRecordController
    {
        private readonly string format = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"";
        private readonly int _pid;
        private readonly string _name;

        private string _filePath;
        private Process _recordTargetProcess;

        private Timer _timer;

        private object _locker = new();
        public RamMonitorRecordController(ProcessDetailModel model)
        {
            _pid = model.Pid;
            _name = model.Name;
        }

        public void Begin()
        {
            _recordTargetProcess = Process.GetProcessById(_pid);
            var now = DateTime.Now;
            var model = new RecordRamMemoryDataModel
            {
                WorkingSet = _recordTargetProcess.WorkingSet64,
                PeakWorkingSet = _recordTargetProcess.PeakWorkingSet64,
                Paged = _recordTargetProcess.PagedMemorySize64,
                PeakPaged = _recordTargetProcess.PeakPagedMemorySize64,
                Virtual = _recordTargetProcess.VirtualMemorySize64,
                PeakVirtual = _recordTargetProcess.PeakVirtualMemorySize64,
                Private = _recordTargetProcess.PrivateMemorySize64,

                RecordTime = now
            };

            _filePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{_name}_RECORD_{now.ToString("yyyyMMddHHmmss")}.csv");

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, "RecordTime", "WorkingSet", "PeakWorkingSet", "Paged", "PeakPaged", "Virtual", "PeakVirtual", "Private", "Note"));
                    sw.WriteLine(string.Format(format, model.RecordTime, model.WorkingSet, model.PeakWorkingSet, model.Paged, model.PeakPaged, model.Virtual, model.PeakVirtual, model.Private, "記録開始"));
                }
            }

            _timer = new(new TimerCallback(_Proc), null, 0, 1000);
        }

        public void End()
        {
            var now = DateTime.Now;
            var model = new RecordRamMemoryDataModel
            {
                WorkingSet = _recordTargetProcess.WorkingSet64,
                PeakWorkingSet = _recordTargetProcess.PeakWorkingSet64,
                Paged = _recordTargetProcess.PagedMemorySize64,
                PeakPaged = _recordTargetProcess.PeakPagedMemorySize64,
                Virtual = _recordTargetProcess.VirtualMemorySize64,
                PeakVirtual = _recordTargetProcess.PeakVirtualMemorySize64,
                Private = _recordTargetProcess.PrivateMemorySize64,

                RecordTime = now
            };

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, model.RecordTime, model.WorkingSet, model.PeakWorkingSet, model.Paged, model.PeakPaged, model.Virtual, model.PeakVirtual, model.Private, "記録終了"));
                }
            }

            _timer.Dispose();
        }

        private void _Proc(object state)
        {
            var now = DateTime.Now;
            var model = new RecordRamMemoryDataModel
            {
                WorkingSet = _recordTargetProcess.WorkingSet64,
                PeakWorkingSet = _recordTargetProcess.PeakWorkingSet64,
                Paged = _recordTargetProcess.PagedMemorySize64,
                PeakPaged = _recordTargetProcess.PeakPagedMemorySize64,
                Virtual = _recordTargetProcess.VirtualMemorySize64,
                PeakVirtual = _recordTargetProcess.PeakVirtualMemorySize64,
                Private = _recordTargetProcess.PrivateMemorySize64,

                RecordTime = now
            };

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, model.RecordTime, model.WorkingSet, model.PeakWorkingSet, model.Paged, model.PeakPaged, model.Virtual, model.PeakVirtual, model.Private, "毎秒記録"));
                }
            }
        }
    }
}
