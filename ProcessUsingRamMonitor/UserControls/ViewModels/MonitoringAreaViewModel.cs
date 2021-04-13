using OxyPlot;
using Prism.Commands;
using Prism.Mvvm;
using ProcessUsingRamMonitor.UserControls.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessUsingRamMonitor.UserControls.ViewModels
{
    public class MonitoringAreaViewModel : BindableBase
    {
        private MonitoringAreaModel _model = new();

        public ReadOnlyReactiveCollection<MemoryDataModel> MemoryDatas { get; }

        public ReactiveProperty<PlotModel> PlotModel { get; } = new();
        public ReactiveProperty<PlotController> Controller { get; } = new();


        public MonitoringAreaViewModel()
        {
            MemoryDatas = _model.RamMonitorDatas.ToReadOnlyReactiveCollection();
        }

        private void Init()
        {
            _model.RamMonitorDatas.Add(new MemoryDataModel { Time = new DateTime(1990, 12, 1, 0, 0, 0), Value = 0});
            _model.RamMonitorDatas.Add(new MemoryDataModel { Time = new DateTime(1990, 12, 1, 0, 0, 1), Value = 0});
            _model.RamMonitorDatas.Add(new MemoryDataModel { Time = new DateTime(1990, 12, 1, 0, 0, 2), Value = 0});
            _model.RamMonitorDatas.Add(new MemoryDataModel { Time = new DateTime(1990, 12, 1, 0, 0, 3), Value = 0});
            _model.RamMonitorDatas.Add(new MemoryDataModel { Time = new DateTime(1990, 12, 1, 0, 0, 4), Value = 0});


            PlotModel.Value.Title = "PlotView";

            // 軸の初期化
            _model.X.Position = OxyPlot.Axes.AxisPosition.Bottom;
            _model.Y.Position = OxyPlot.Axes.AxisPosition.Left;

            // 線グラフ
            _model.LineSeries = new OxyPlot.Series.LineSeries();
            _model.LineSeries.Title = "Custom";
            _model.LineSeries.ItemsSource = MemoryDatas;
            _model.LineSeries.DataFieldX = nameof(MemoryDataModel.Time);
            _model.LineSeries.DataFieldY = nameof(MemoryDataModel.Value);

            var a = 1;
            var b = 2;

            // 関数グラフ
            _model.FunctionSeries = new OxyPlot.Series.FunctionSeries
            (
                x => a * x + b, 0, 30, 5, "Y = ax + b"
            );

            PlotModel.Value.Axes.Add(_model.X);
            PlotModel.Value.Axes.Add(_model.Y);
            PlotModel.Value.Series.Add(_model.LineSeries);
            PlotModel.Value.Series.Add(_model.FunctionSeries);

            PlotModel.Value.InvalidatePlot(true);
        }

        private void InitController()
        {
            // グラフのマウス操作およびキー操作の初期化
            Controller.Value.UnbindKeyDown(OxyKey.A);
            Controller.Value.UnbindKeyDown(OxyKey.C, OxyModifierKeys.Control);
            Controller.Value.UnbindKeyDown(OxyKey.C, OxyModifierKeys.Control | OxyModifierKeys.Alt);
            Controller.Value.UnbindKeyDown(OxyKey.R, OxyModifierKeys.Control | OxyModifierKeys.Alt);
            Controller.Value.UnbindTouchDown();

            Controller.Value.UnbindMouseDown(OxyMouseButton.Left);
            Controller.Value.UnbindMouseDown(OxyMouseButton.Middle);
            Controller.Value.UnbindMouseDown(OxyMouseButton.Right);

            Controller.Value.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            Controller.Value.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PointsOnlyTrack);
            Controller.Value.BindMouseDown(OxyMouseButton.Right, PlotCommands.ZoomRectangle);
        }

    }
}
