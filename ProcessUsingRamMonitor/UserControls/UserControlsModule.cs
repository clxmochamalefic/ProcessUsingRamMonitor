using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace ProcessUsingRamMonitor.UserControls
{
    public class UserControlsModule : IModule
    {
        [Dependency]
        public IRegionManager RegionManager { get; set; }
        [Dependency]
        public IUnityContainer Container { get; set; }

        public UserControlsModule()
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.RegionManager.RegisterViewWithRegion("ProcessRegion", typeof(UserControls.Views.ProcessAreaView));
            this.RegionManager.RegisterViewWithRegion("MonitoringRegion", typeof(UserControls.Views.MonitoringAreaView));
        }
    }
}
