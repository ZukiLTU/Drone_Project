using DJI.WindowsSDK;
using DJI.WindowsSDK.Mission.Waypoint;
using DJIDrone.Commandes;
using DJIDrone.ViewModels;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace DJIDrone.ViewModels
{
    class WaypointMissionViewModel : ViewModelBase
    {
        List<Waypoint> ptsDePassage;

        private static readonly WaypointMissionViewModel _singleton = new WaypointMissionViewModel();
        public static WaypointMissionViewModel Instance
        {
            get
            {
                return _singleton;
            }
        }

        private WaypointMissionViewModel()
        {
            DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).IsSimulatorStartedChanged += WaypointMission_IsSimulatorStartedChanged;
            DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).StateChanged += WaypointMission_StateChanged;
            DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AltitudeChanged += WaypointMission_AltitudeChanged; ;
            DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).AircraftLocationChanged += WaypointMission_AircraftLocationChanged; ;
            WaypointMissionState = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).GetCurrentState();
        }

        private async void WaypointMission_AircraftLocationChanged(object sender, LocationCoordinate2D? value)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (value.HasValue)
                {
                    AircraftLocation = value.Value;
                }
            });
        }

        private async void WaypointMission_AltitudeChanged(object sender, DoubleMsg? value)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (value.HasValue)
                {
                    AircraftAltitude = value.Value.value;
                }
            });
        }

        private async void WaypointMission_StateChanged(WaypointMissionHandler sender, WaypointMissionStateTransition? value)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                WaypointMissionState = value.HasValue ? value.Value.current : WaypointMissionState.UNKNOWN;
            });
        }

        private async void WaypointMission_IsSimulatorStartedChanged(object sender, BoolMsg? value)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsSimulatorStart = value.HasValue && value.Value.value;
            });
        }
        public String Latitude { get; set; }
        public String Longitude { get; set; }

        public String SimulatorLatitude { set; get; }
        public String SimulatorLongitude { set; get; }
        public String SimulatorSatelliteCount { set; get; }
        public String Latitude1 { set; get; }
        public String Latitude2 { set; get; }
        public String Latitude3 { set; get; }
        public String Latitude4 { set; get; }
        public String Longitude1 { set; get; }
        public String Longitude2 { set; get; }
        public String Longitude3 { set; get; }
        public String Longitude4 { set; get; }
        bool _isSimulatorStart = false;

        /*public void GetCoords()
        {
            ptsDePassage.Add();
        }*/
        public bool IsSimulatorStart
        {
            get
            {
                return _isSimulatorStart;
            }
            set
            {
                _isSimulatorStart = value;
                OnPropertyChanged(nameof(IsSimulatorStart));
                OnPropertyChanged(nameof(SimulatorState));
            }
        }
        public String SimulatorState
        {
            get
            {
                return _isSimulatorStart ? "Ouvert" : "Fermé";
            }
        }
        private WaypointMissionState _waypointMissionState;
        public WaypointMissionState WaypointMissionState
        {
            get
            {
                return _waypointMissionState;
            }
            set
            {
                _waypointMissionState = value;
                OnPropertyChanged(nameof(WaypointMissionState));
            }
        }


        private double _aircraftAltitude = 0;
        public double AircraftAltitude
        {
            get
            {
                return _aircraftAltitude;
            }
            set
            {
                _aircraftAltitude = value;
                OnPropertyChanged(nameof(AircraftAltitude));
            }
        }
        public ICommand _startSimulator;
        public ICommand StartSimulator
        {
            get
            {
                if (_startSimulator == null)
                {
                    _startSimulator = new Relais(async delegate ()
                    {
                        try
                        {
                            var latitude = Convert.ToDouble(SimulatorLatitude);
                            var longitude = Convert.ToDouble(SimulatorLongitude);
                            var satelliteCount = 4;

                            var err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StartSimulatorAsync(new SimulatorInitializationSettings
                            {
                                latitude = latitude,
                                longitude = longitude,
                                satelliteCount = satelliteCount
                            });
                            var messageDialog = new MessageDialog(String.Format("Résultat du début: {0}.", err.ToString()));
                            await messageDialog.ShowAsync();
                        }
                        catch
                        {
                            var messageDialog = new MessageDialog("Erreur de format !");
                            await messageDialog.ShowAsync();
                        }
                    }, delegate () { return true; });
                }
                return _startSimulator;
            }
        }

        public ICommand _stopSimulator;
        public ICommand StopSimulator
        {
            get
            {
                if (_stopSimulator == null)
                {
                    _stopSimulator = new Relais(async delegate ()
                    {
                        var err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).StopSimulatorAsync();
                        var messageDialog = new MessageDialog(String.Format("Résultat de l'arrêt: {0}.", err.ToString()));
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _stopSimulator;
            }
        }

        private WaypointMission _waypointMission;
        public WaypointMission WaypointMission
        {
            get { return _waypointMission; }
            set
            {
                _waypointMission = value;
                OnPropertyChanged(nameof(WaypointMission));
            }
        }

        private LocationCoordinate2D _aircraftLocation = new LocationCoordinate2D() { latitude = 0, longitude = 0 };
        public LocationCoordinate2D AircraftLocation
        {
            get
            {
                return _aircraftLocation;
            }
            set
            {
                _aircraftLocation = value;
                OnPropertyChanged(nameof(AircraftLocation));
            }
        }

        private Waypoint InitDumpWaypoint(double latitude, double longitude)
        {
            Waypoint waypoint = new Waypoint()
            {
                location = new LocationCoordinate2D() { latitude = latitude, longitude = longitude },
                altitude = 20,
                gimbalPitch = -30,
                turnMode = WaypointTurnMode.CLOCKWISE,
                heading = 0,
                actionRepeatTimes = 1,
                actionTimeoutInSeconds = 60,
                cornerRadiusInMeters = 0.2,
                speed = 0,
                shootPhotoTimeInterval = -1,
                shootPhotoDistanceInterval = -1,
                waypointActions = new List<WaypointAction>()
            };
            return waypoint;
        }

        public ICommand _initWaypointMission;
        public ICommand InitWaypointMission
        {
            get
            {
                if (_initWaypointMission == null)
                {
                    _initWaypointMission = new Relais(delegate ()
                    {
                        double nowLat = AircraftLocation.latitude;
                        double nowLng = AircraftLocation.longitude;
                        WaypointMission mission = new WaypointMission()
                        {
                            waypointCount = 0,
                            maxFlightSpeed = 15,
                            autoFlightSpeed = 10,
                            finishedAction = WaypointMissionFinishedAction.NO_ACTION,
                            headingMode = WaypointMissionHeadingMode.AUTO,
                            flightPathMode = WaypointMissionFlightPathMode.NORMAL,
                            gotoFirstWaypointMode = WaypointMissionGotoFirstWaypointMode.SAFELY,
                            exitMissionOnRCSignalLostEnabled = false,
                            pointOfInterest = new LocationCoordinate2D()
                            {
                                latitude = 0,
                                longitude = 0
                            },
                            gimbalPitchRotationEnabled = true,
                            repeatTimes = 0,
                            missionID = 0,
                            waypoints = new List<Waypoint>()
                            {
                                /*
                                InitDumpWaypoint(Convert.ToDouble(Latitude1), Convert.ToDouble(Longitude1)),
                                InitDumpWaypoint(Convert.ToDouble(Latitude2), Convert.ToDouble(Longitude2)),
                                InitDumpWaypoint(Convert.ToDouble(Latitude3), Convert.ToDouble(Longitude3)),
                                InitDumpWaypoint(Convert.ToDouble(Latitude4), Convert.ToDouble(Longitude4)),*/
                                InitDumpWaypoint(nowLat+0.0001, nowLng+0.00015),
                                InitDumpWaypoint(nowLat+0.0001, nowLng-0.00015),
                                InitDumpWaypoint(nowLat-0.0001, nowLng-0.00015),
                                InitDumpWaypoint(nowLat-0.0001, nowLng+0.00015),
                                //InitDumpWaypoint(Convert.ToDouble(Latitude), Convert.ToDouble(Longitude))
                            }
                        };
                        WaypointMission = mission;
                    }, delegate () { return true; });
                }
                return _initWaypointMission;
            }
        }

        public ICommand _addAction;
        public ICommand AddAction
        {
            get
            {
                if (_addAction == null)
                {
                    _addAction = new Relais(async delegate ()
                    {
                        String dialogMsg = "";
                        do
                        {
                            if (WaypointMission.waypoints.Count < 2)
                            {
                                dialogMsg = "Mission non initié, veuillez l'initier !";
                                break;
                            }
                            WaypointMission.waypoints[1].waypointActions.Add(new WaypointAction() { actionType = WaypointActionType.STAY, actionParam = 2000 });
                            dialogMsg = "Succès ! Le drone va rester sur le point pendant 2000ms.";
                        } while (false);
                        var messageDialog = new MessageDialog(dialogMsg);
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _addAction;
            }
        }
        public ICommand _loadMission;
        public ICommand LoadMission
        {
            get
            {
                if (_loadMission == null)
                {
                    _loadMission = new Relais(async delegate ()
                    {
                        SDKError err = DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).LoadMission(this.WaypointMission);
                        var messageDialog = new MessageDialog(String.Format("Mission de chargement du SDK: {0}", err.ToString()));
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _loadMission;
            }
        }
        public ICommand _setGroundStationModeEnabled;
        public ICommand SetGroundStationModeEnabled
        {
            get
            {
                if (_setGroundStationModeEnabled == null)
                {
                    _setGroundStationModeEnabled = new Relais(async delegate ()
                    {
                        SDKError err = await DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0).SetGroundStationModeEnabledAsync(new BoolMsg() { value = true });
                        var messageDialog = new MessageDialog(String.Format("Mode stationement au sol actif: {0}", err.ToString()));
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _setGroundStationModeEnabled;
            }
        }
        public ICommand _uploadMission;
        public ICommand UploadMission
        {
            get
            {
                if (_uploadMission == null)
                {
                    _uploadMission = new Relais(async delegate ()
                    {
                        SDKError err = await DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).UploadMission();
                        var messageDialog = new MessageDialog(String.Format("Transférer la mission au drone: {0}", err.ToString()));
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _uploadMission;
            }
        }
        public ICommand _startMission;
        public ICommand StartMission
        {
            get
            {
                if (_startMission == null)
                {
                    _startMission = new Relais(async delegate ()
                    {
                        var err = await DJISDKManager.Instance.WaypointMissionManager.GetWaypointMissionHandler(0).StartMission();
                        var messageDialog = new MessageDialog(String.Format("Début de mission: {0}", err.ToString()));
                        await messageDialog.ShowAsync();
                    }, delegate () { return true; });
                }
                return _startMission;
            }
        }
    }
}