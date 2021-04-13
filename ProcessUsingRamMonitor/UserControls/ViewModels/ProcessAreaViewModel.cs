using Prism.Commands;
using Prism.Mvvm;
using ProcessUsingRamMonitor.Controllers;
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

        private bool _isRecording;

        private RamMonitorRecordController _controller;

        public ReadOnlyReactiveCollection<ProcessDetailModel> Processes { get; internal set; }

        public ReactiveProperty<ProcessDetailModel> SelectedProcess { get; } = new();

        public ReactiveProperty<string> FindProcessName { get; set; } = new();

        public ReactiveProperty<string> RecordingStatus { get; } = new();

        public ReactiveCommand ReloadCommand { get; } = new();

        public ReactiveCommand RecordCommand { get; } = new();
        public ProcessAreaViewModel()
        {
            Processes = _model.ProcessDetails.ToReadOnlyReactiveCollection();
            FindProcessName.Subscribe(x => _ModifyFindProcessName(x));
            RecordingStatus.Subscribe(x => _model.RecordingStatus = x);
            SelectedProcess.Subscribe(x => _model.SelectedProcess = x);

            ReloadCommand.Subscribe(() => _ReloadAction());
            RecordCommand.Subscribe(() =>
            {
                _isRecording = !_isRecording;
                RecordingStatus.Value = _isRecording ? "Stop" : "Start";

                if (_isRecording)
                {
                    _controller = new(new()
                    {
                        Pid = _model.SelectedProcess.Pid,
                        Name = _model.SelectedProcess.Name
                    });

                    _controller.Begin();
                }
                else
                {
                    _controller.End();
                }
            });
            RecordingStatus.Value = "Start";
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
            RecordingStatus.Dispose();

            ReloadCommand.Dispose();
            RecordCommand.Dispose();
        }
    }
}
