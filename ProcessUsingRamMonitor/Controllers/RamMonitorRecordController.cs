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

        private Timer _timer;

        private object _locker = new();
        public RamMonitorRecordController(ProcessDetailModel model)
        {
            _pid = model.Pid;
            _name = model.Name;
        }

        public void Begin()
        {
            var data = _GetMemoryData(_pid);

            if (data == null)
            {
                return;
            }

            _filePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{_name}_RECORD_{data.RecordTime.ToString("yyyyMMddHHmmss")}.csv");

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, "RecordTime", "WorkingSet", "PeakWorkingSet", "Paged", "PeakPaged", "Virtual", "PeakVirtual", "Private", "Note"));
                    sw.WriteLine(string.Format(format, data.RecordTime, data.WorkingSet, data.PeakWorkingSet, data.Paged, data.PeakPaged, data.Virtual, data.PeakVirtual, data.Private, "記録開始"));
                }
            }

            _timer = new(new TimerCallback(_Proc), null, 0, 1000);
        }

        public void End()
        {
            var data = _GetMemoryData(_pid);

            if (data == null)
            {
                return;
            }

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, data.RecordTime, data.WorkingSet, data.PeakWorkingSet, data.Paged, data.PeakPaged, data.Virtual, data.PeakVirtual, data.Private, "記録終了"));
                }
            }

            _timer.Dispose();
        }

        private void _Proc(object state)
        {
            var data = _GetMemoryData(_pid);

            if (data == null)
            {
                return;
            }

            lock (_locker)
            {
                using (var sw = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format(format, data.RecordTime, data.WorkingSet, data.PeakWorkingSet, data.Paged, data.PeakPaged, data.Virtual, data.PeakVirtual, data.Private, "毎秒記録"));
                }
            }
        }

        private RecordRamMemoryDataModel _GetMemoryData(int pid)
        {
            if (pid == 0)
            {
                return null;
            }

            Process process = null;
            try
            {
                process = Process.GetProcessById(pid);

                if (process.HasExited)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            if (process == null)
            {
                return null;
            }

            var now = DateTime.Now;
            var model = new RecordRamMemoryDataModel
            {
                WorkingSet = process.WorkingSet64,
                PeakWorkingSet = process.PeakWorkingSet64,
                Paged = process.PagedMemorySize64,
                PeakPaged = process.PeakPagedMemorySize64,
                Virtual = process.VirtualMemorySize64,
                PeakVirtual = process.PeakVirtualMemorySize64,
                Private = process.PrivateMemorySize64,

                RecordTime = now
            };

            return model;
        }
    }
}
