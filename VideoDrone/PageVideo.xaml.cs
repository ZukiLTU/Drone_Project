using DJI.WindowsSDK;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DJIVideoParser;
using DJI.WindowsSDK.Components;
using System;

namespace DJIDrone.VideoDrone
{
    public sealed partial class PageVideo : Page
    {
        private Parser videoParser;
        private FlightControllerHandler flightControllerHandler;
        private RemoteControllerHandler remote;
        private CommandesPilotage commandes = new CommandesPilotage();
        public PageVideo()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            InitializeVideoFeedModule();
            await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).SetCameraWorkModeAsync(new CameraWorkModeMsg { value = CameraWorkMode.SHOOT_PHOTO });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //UninitializeVideoFeedModule();
        }

        /// <summary>
        /// Initialise les flux vidéo
        /// </summary>
        private async void InitializeVideoFeedModule()
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (videoParser == null)
                {
                    videoParser = new Parser();
                    videoParser.Initialize(delegate (byte[] data)
                    {
                        return DJISDKManager.Instance.VideoFeeder.ParseAssitantDecodingInfo(0, data);
                    });
                    videoParser.SetSurfaceAndVideoCallback(0, 0, swapChainPanel, ReceiveDecodedData);
                    DJISDKManager.Instance.VideoFeeder.GetPrimaryVideoFeed(0).VideoDataUpdated += OnVideoPush;
                }
                DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).CameraTypeChanged += OnCameraTypeChanged;
                var type = await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).GetCameraTypeAsync();
                OnCameraTypeChanged(this, type.value);
            });
        }

        /// <summary>
        /// le contraire
        /// </summary>
        private void UninitializeVideoFeedModule()
        {
            if (DJISDKManager.Instance.SDKRegistrationResultCode == SDKError.NO_ERROR)
            {
                videoParser.SetSurfaceAndVideoCallback(0, 0, null, null);
                DJISDKManager.Instance.VideoFeeder.GetPrimaryVideoFeed(0).VideoDataUpdated -= OnVideoPush;
            }
        }
        void OnVideoPush(VideoFeed sender, byte[] bytes)
        {
            videoParser.PushVideoData(0, 0, bytes, bytes.Length);
        }

        /// <summary>
        /// Retourne un tableau de bits en RGBA
        /// </summary>
        /// <param name="data"> tableau de bits </param>
        /// <param name="width">largeur</param>
        /// <param name="height">hauteur</param>
        async void ReceiveDecodedData(byte[] data, int width, int height)
        {
        }

        /// <summary>
        /// Changement caméra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void OnCameraTypeChanged(object sender, CameraTypeMsg? value)
        {
            if (value != null)
            {
                switch (value.Value.value)
                {
                    case CameraType.MAVIC_2_ZOOM:
                        this.videoParser.SetCameraSensor(AircraftCameraType.Mavic2Zoom);
                        break;
                    case CameraType.MAVIC_2_PRO:
                        this.videoParser.SetCameraSensor(AircraftCameraType.Mavic2Pro);
                        break;
                    default:
                        this.videoParser.SetCameraSensor(AircraftCameraType.Others);
                        break;
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //lblAltitude1.Text = "Altitude : " + flightControllerHandler.GetAltitudeAsync().ToString() + "m";
            commandes.Avancer();
        }
    }
}