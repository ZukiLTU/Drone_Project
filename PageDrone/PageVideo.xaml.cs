using DJI.WindowsSDK;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DJIVideoParser;
using DJI.WindowsSDK.Components;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace DJIDrone.VideoDrone
{
    public sealed partial class PageVideo : Page
    {
        public Parser videoParser;
        public FlightControllerHandler flightControllerHandler;
        public RemoteControllerHandler remote;
        public CommandesPilotage commandes = new CommandesPilotage();
        public PageVideo()
        {
            this.InitializeComponent();
            //flightControllerHandler.AltitudeChanged += FlightControllerHandler_AltitudeChanged;
        }

        private async void FlightControllerHandler_AltitudeChanged(object sender, DoubleMsg? value)
        {
            if (flightControllerHandler != null)
            {
                lblAltitude1.Text = "Altitude : " + flightControllerHandler.GetAltitudeAsync().ToString() + "m";
            }
            else return;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*if(flightControllerHandler != null) 
            { 
                var a = flightControllerHandler.GetIsFlyingAsync();
                lblVol.Text = "Mode vol :" + a.ToString();

                var b = flightControllerHandler.GetAltitudeAsync();
                lblAltitude1.Text = "Altitude : " + b.ToString();

                var c = flightControllerHandler.GetFlightTimeInSecondsAsync();
                lblTpsVol.Text = "Temps de vol : " + c.ToString() + "s";

                var d = flightControllerHandler.GetHeightLimitAsync();
                lblHauteurLimite.Text = "Hauteur de vol : " + d.ToString() + "m";
            }*/
        }

        private async void btnAfficherVid_Click(object sender, RoutedEventArgs e)
        {
            var viewId = 0;

            var newView = CoreApplication.CreateNewView();
            await newView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(Video.Video));
                    Window.Current.Content = frame;

                    viewId = ApplicationView.GetForCurrentView().Id;

                    //ApplicationView.GetForCurrentView().Consolidated += App.ViewConsolidated;

                    Window.Current.Activate();
                });

            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewId);
        }
    }
}