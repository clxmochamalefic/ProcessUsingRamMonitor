using OxyPlot;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessUsingRamMonitor.UserControls.Models
{
    public class MonitoringAreaModel : BindableBase
    {
        private ObservableCollection<MemoryDataModel> _ramMonitorDatas = new();
        public ObservableCollection<MemoryDataModel> RamMonitorDatas
        {
            get { return _ramMonitorDatas; }
            set { SetProperty(ref _ramMonitorDatas, value); }
        }

        private PlotModel _plotModel = new PlotModel();
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { SetProperty(ref _plotModel, value); }
        }

        private PlotController _plotController = new();
        public PlotController Controller
        {
            get { return _plotController; }
            set { SetProperty(ref _plotController, value); }
        }

        public OxyPlot.Axes.TimeSpanAxis X { get; } = new OxyPlot.Axes.TimeSpanAxis();
        public OxyPlot.Axes.LinearAxis Y { get; } = new OxyPlot.Axes.LinearAxis();
        public OxyPlot.Series.LineSeries LineSeries { get; internal set; }
        public OxyPlot.Series.FunctionSeries FunctionSeries { get; internal set; }

    }
}
