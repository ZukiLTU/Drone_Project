using DJI.WindowsSDK;
using DJI.WindowsSDK.FlySafe;

using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace DJIDrone.Vol
{

    public sealed partial class PageVol : Page
    {
        public PageVol()
        {
            this.InitializeComponent();
        }
        private void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            string chemin = "https://www.geoportail.gouv.fr/embed/visu.html?c=1.22772216796875,49.31706575342582&z=10&l0=GEOGRAPHICALGRIDSYSTEMS.MAPS.SCAN25TOUR::GEOPORTAIL:OGC:WMTS(1)&l1=TRANSPORTS.DRONES.RESTRICTIONS::GEOPORTAIL:OGC:WMS(1)&permalink=yes";
            Uri uri = new Uri(chemin);
            wb.Source = uri;
        }
    }
}