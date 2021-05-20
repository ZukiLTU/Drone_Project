using System;
using System.Collections.Generic;
using System.Linq;
using DJI.WindowsSDK;
using DJIDrone.ViewModels;
using DJIDrone.Vol;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;


namespace DJIDrone.WaypointHandling
{
    public sealed class OperationException : Exception
    {
        public OperationException(String message, SDKError error) : base(String.Format(message))
        {
        }
    }
    public sealed partial class PagePoints : Page
    {
        PageVol page = new PageVol();
        private MapIcon aircraftMapIcon = null;
        MapElementsLayer routeLayer = new MapElementsLayer();
        MapElementsLayer waypointLayer = new MapElementsLayer();
        MapElementsLayer locationLayer = new MapElementsLayer();
        WaypointMissionViewModel waypointMissionViewModel;
        public PagePoints()
        {
            this.InitializeComponent();
            //Couches de la carte
            WaypointMap.Layers.Add(routeLayer);
            WaypointMap.Layers.Add(waypointLayer);
            WaypointMap.Layers.Add(locationLayer);
            WaypointMissionViewModel.Instance.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(WaypointMissionViewModel.WaypointMission)))
            {
                RedrawWaypoint();
            }
            else if (e.PropertyName.Equals(nameof(WaypointMissionViewModel.AircraftLocation)))
            {
                var value = WaypointMissionViewModel.Instance.AircraftLocation;
                AircraftLocationChange(value);
            }
        }

        /// <summary>
        /// Fonction qui fait bouger la localisation du drone.
        /// </summary>
        /// <param name="value">Valeur de la postion 2D</param>
        private void AircraftLocationChange(LocationCoordinate2D value)
        {
            if (aircraftMapIcon == null)
            {
                aircraftMapIcon = new MapIcon()
                {
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    ZIndex = 1,
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/SmallTile.scale-100.png")),
                };
                locationLayer.MapElements.Add(aircraftMapIcon);
            }
            aircraftMapIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = value.latitude, Longitude = value.longitude });
            coords.Text = Convert.ToString(value.longitude) + ";" + Convert.ToString(value.latitude);
        }

        ~PagePoints()
        {
        }

        /// <summary>
        /// Déctecte quand le drone a navigué vers un endroit.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = WaypointMissionViewModel.Instance;
            base.OnNavigatedTo(e);
            GetIfInSimulation();
        }
        /// <summary>
        /// Détecte si on est en simulation ou non
        /// </summary>
        private void GetIfInSimulation()
        {
            var isInSimulationValue = WaypointMissionViewModel.Instance.IsSimulatorStart;
            if (isInSimulationValue)
            {
                var aircraftLocaton = WaypointMissionViewModel.Instance.AircraftLocation;
                WaypointMap.Center = new Geopoint((new BasicGeoposition() { Latitude = aircraftLocaton.latitude, Longitude = aircraftLocaton.longitude }));
                AircraftLocationChange(aircraftLocaton);
            }
        }

        /// <summary>
        /// Redessine le point de passage
        /// </summary>
        private void RedrawWaypoint()
        {
            List<BasicGeoposition> waypointPositions = new List<BasicGeoposition>();
            WaypointMission mission = WaypointMissionViewModel.Instance.WaypointMission;
            for (int i = 0; i < mission.waypoints.Count(); ++i)
            {
                if (waypointLayer.MapElements.Count == i)
                {
                    MapIcon waypointIcon = new MapIcon()
                    {
                        Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/waypoint.png")),
                        NormalizedAnchorPoint = new Point(0.5, 0.5),
                        ZIndex = 0,
                    };
                    waypointLayer.MapElements.Add(waypointIcon);
                }

                var geolocation = new BasicGeoposition() { Latitude = mission.waypoints[i].location.latitude, Longitude = mission.waypoints[i].location.longitude };
                (waypointLayer.MapElements[i] as MapIcon).Location = new Geopoint(geolocation);
                waypointPositions.Add(geolocation);
            }
            if (routeLayer.MapElements.Count == 0 && waypointPositions.Count >= 2)
            {
                var polyline = new MapPolyline
                {
                    StrokeColor = Color.FromArgb(255, 0, 255, 0),
                    Path = new Geopath(waypointPositions),
                    StrokeThickness = 2
                };
                routeLayer.MapElements.Add(polyline);
            }
            else
            {
                var waypointPolyline = routeLayer.MapElements[0] as MapPolyline;
                waypointPolyline.Path = new Geopath(waypointPositions);
            }

        }

        private void TBlock_SelectionChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //rien lol.
        }

        private async void WaypointMap_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await InitMapAsync();
        }
        public async System.Threading.Tasks.Task InitMapAsync()
        {
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator();
                Geoposition pos = await geolocator.GetGeopositionAsync();
                Geopoint myLocation = pos.Coordinate.Point;
                // Met la location du utilisateur 
                WaypointMap.Center = myLocation;
                WaypointMap.ZoomLevel = 8;
                WaypointMap.LandmarksVisible = true;
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
