using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using ProcessUsingRamMonitor.UserControls.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace ProcessUsingRamMonitor.UserControls.ViewModels
{
    public class MonitoringAreaViewModel : BindableBase, INavigationAware, IDisposable
    {
        private MonitoringAreaModel _model = new();

        public SeriesCollection SeriesCollection { get; set; } = new();

        public List<string> LabelsBase { get; set; } = new();

        public ReadOnlyReactiveCollection<string> Labels;

        public Func<double, string> YFormatter { get; set; }


        public ReactivePropertySlim<int> Pid { get; } = new();

        public ReactivePropertySlim<string> Name { get; } = new();

        public ReadOnlyReactiveCollection<MemoryDataModel> MemoryDatas { get; }

        private ReactiveTimer _timer = new(TimeSpan.FromSeconds(1));

        private object _locker = new();

        private ChartValues<long> workingSetValues = new ChartValues<long>();
        private ChartValues<long> peakWorkingSetValues = new ChartValues<long>();
        private ChartValues<long> pagedValues = new ChartValues<long>();
        private ChartValues<long> peakPagedValues = new ChartValues<long>();
        private ChartValues<long> virtualValues = new ChartValues<long>();
        private ChartValues<long> peakVirtualValues = new ChartValues<long>();
        private ChartValues<long> privateValues = new ChartValues<long>();

        public MonitoringAreaViewModel()
        {
            Pid.Subscribe(x => _model.Pid = x);
            Name.Subscribe(x => _model.Name = x);

            Labels = _model.RamMonitorDatas.Select(x => x.Time.ToString("HHmmss")).ToObservable().ToReadOnlyReactiveCollection();

            MemoryDatas = _model.RamMonitorDatas.ToReadOnlyReactiveCollection();
            _timer.Subscribe(x =>
            {
                lock (_locker)
                {
                    if (Pid.Value == 0)
                    {
                        return;
                    }

                    Process process = null;
                    try
                    {
                        process = Process.GetProcessById(Pid.Value);

                        if (process.HasExited)
                        {
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (process == null)
                    {
                        return;
                    }

                    if (_model.RamMonitorDatas.Count() >= 60 * 7)
                    {
                        var removeWorkingSet = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.WorkingSet).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removeWorkingSet);
                        var removePeakWorkingSet = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.PeakWorkingSet).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removePeakWorkingSet);
                        var removePaged = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.Paged).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removePaged);
                        var removePeakPaged = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.PeakPaged).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removePeakPaged);
                        var removeVirtual = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.Virtual).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removeVirtual);
                        var removePeakVirtual = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.PeakVirtual).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removePeakVirtual);
                        var removePrivate = _model.RamMonitorDatas.Where(x => x.MemoryType == MemoryType.Private).OrderBy(x => x.Time).FirstOrDefault();
                        _model.RamMonitorDatas.Remove(removePrivate);

                        workingSetValues.RemoveAt(0);
                        peakWorkingSetValues.RemoveAt(0);
                        pagedValues.RemoveAt(0);
                        peakPagedValues.RemoveAt(0);
                        virtualValues.RemoveAt(0);
                        peakVirtualValues.RemoveAt(0);
                        privateValues.RemoveAt(0);
                    }

                    var newWorkingSet = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.WorkingSet64,
                        MemoryType = MemoryType.WorkingSet
                    };
                    _model.RamMonitorDatas.Add(newWorkingSet);

                    var newPeakWorkingSet = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.PeakWorkingSet64,
                        MemoryType = MemoryType.PeakWorkingSet
                    };
                    _model.RamMonitorDatas.Add(newPeakWorkingSet);

                    var newPaged = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.PagedMemorySize64,
                        MemoryType = MemoryType.Paged
                    };
                    _model.RamMonitorDatas.Add(newPaged);

                    var newPeakPaged = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.PeakPagedMemorySize64,
                        MemoryType = MemoryType.PeakPaged
                    };
                    _model.RamMonitorDatas.Add(newPeakPaged);

                    var newVirutal = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.VirtualMemorySize64,
                        MemoryType = MemoryType.Virtual
                    };
                    _model.RamMonitorDatas.Add(newVirutal);

                    var newPeakVirtual = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.PeakVirtualMemorySize64,
                        MemoryType = MemoryType.PeakVirtual
                    };
                    _model.RamMonitorDatas.Add(newPeakVirtual);

                    var newPrivate = new MemoryDataModel()
                    {
                        Time = DateTime.Now,
                        Value = process.PrivateMemorySize64,
                        MemoryType = MemoryType.Private
                    };
                    _model.RamMonitorDatas.Add(newPrivate);


                    LabelsBase.Clear();

                    workingSetValues.Add(newWorkingSet.Value);
                    peakWorkingSetValues.Add(newPeakWorkingSet.Value);
                    pagedValues.Add(newPaged.Value);
                    peakPagedValues.Add(newPeakPaged.Value);
                    virtualValues.Add(newVirutal.Value);
                    peakVirtualValues.Add(newPeakVirtual.Value);
                    privateValues.Add(newPrivate.Value);
                }
            });
            _timer.Start();

            SeriesCollection.Add(new LineSeries()
            {
                Values = workingSetValues,
                Title = MemoryType.WorkingSet.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = peakWorkingSetValues,
                Title = MemoryType.PeakWorkingSet.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = pagedValues,
                Title = MemoryType.Paged.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = peakPagedValues,
                Title = MemoryType.PeakPaged.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = virtualValues,
                Title = MemoryType.Virtual.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = peakVirtualValues,
                Title = MemoryType.PeakVirtual.ToString()
            });
            SeriesCollection.Add(new LineSeries()
            {
                Values = privateValues,
                Title = MemoryType.Private.ToString()
            });

            YFormatter = value => value.ToString();
        }

        public void Dispose()
        {
            Pid.Dispose();
            Name.Dispose();

            MemoryDatas.Dispose();
            _timer.Dispose();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var pid = (int)navigationContext.Parameters["pid"];
            var name = navigationContext.Parameters["name"].ToString();

            Pid.Value = pid;
            Name.Value = name;

            _model.RamMonitorDatas.Clear();

            workingSetValues.Clear();
            peakWorkingSetValues.Clear();
            pagedValues.Clear();
            peakPagedValues.Clear();
            virtualValues.Clear();
            peakVirtualValues.Clear();
            privateValues.Clear();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
