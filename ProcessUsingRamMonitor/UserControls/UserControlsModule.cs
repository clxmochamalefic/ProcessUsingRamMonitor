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
        public UserControlsModule()
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<Login.Views.LoginView>(nameof(Login.Views.LoginView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("ProcessRegion", typeof(UserControls.Views.ProcessAreaView));
            regionManager.RegisterViewWithRegion("MonitoringRegion", typeof(UserControls.Views.MonitoringAreaView));
        }
    }
}
