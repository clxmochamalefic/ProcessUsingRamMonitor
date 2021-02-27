using Prism.Commands;
using Prism.Mvvm;
using ProcessUsingRamMonitor.UserControls.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ProcessUsingRamMonitor.UserControls.ViewModels
{
    public class ProcessAreaViewModel : BindableBase, IDisposable
    {
        private ProcessAreaModel _model = new();
        public ReadOnlyReactiveCollection<ProcessDetailModel> Processes { get; internal set; }

        public ReactiveProperty<string> FindProcessName { get; set; } = new();

        public ReactiveCommand ReloadCommand { get; } = new();
        public ProcessAreaViewModel()
        {
            Processes = _model.ProcessDetails.ToReadOnlyReactiveCollection();
            FindProcessName.Subscribe(x => _ModifyFindProcessName(x));
            ReloadCommand.Subscribe(() => _ReloadAction());
        }

        private void _ModifyFindProcessName(string value)
        {
            _model.FindProcessName = value;
            _ReloadAction();
        }

        private void _ReloadAction()
        {
//            Task.Run(() =>
//            {
                lock (_model)
                {
                    _model.ProcessDetails.Clear();

                    var processes = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();

                    if (!string.IsNullOrWhiteSpace(_model.FindProcessName))
                    {
                        processes = processes.Where(x => x.ProcessName.Contains(_model.FindProcessName)).ToArray();
                    }

                    foreach (var process in processes)
                    {
                        Icon image = null;
                        try
                        {
                            image = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                        }
                        catch (Exception ex)
                        {

                        }

                        _model.ProcessDetails.Add(new()
                        {
                            Pid = process.Id,
                            Name = process.ProcessName,
                            Image = image == null ? null : Imaging.CreateBitmapSourceFromHIcon(image.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                        });
                    }
                }
//            });
        }

        public void Dispose()
        {
            FindProcessName.Dispose();
            Processes.Dispose();
            ReloadCommand.Dispose();
        }
    }
}
