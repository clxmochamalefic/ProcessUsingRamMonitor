using Prism.Ioc;
using Prism.Modularity;
using ProcessUsingRamMonitor.UserControls;
using ProcessUsingRamMonitor.Views;
using System.Windows;

namespace ProcessUsingRamMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<UserControlsModule>(InitializationMode.WhenAvailable);
        }
    }
}
