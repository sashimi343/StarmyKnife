using MahApps.Metro.Controls;

using Prism.Regions;

using StarmyKnife.Constants;

namespace StarmyKnife.Views;

public partial class ShellWindow : MetroWindow
{
    public ShellWindow(IRegionManager regionManager)
    {
        InitializeComponent();
        RegionManager.SetRegionName(hamburgerMenuContentControl, Regions.Main);
        RegionManager.SetRegionManager(hamburgerMenuContentControl, regionManager);
    }
}
