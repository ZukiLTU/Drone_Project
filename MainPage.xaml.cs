/*                                                                    
▀███▀▀▀██▄    ▀████▀████▀███▀▀▀██▄                                      
  ██    ▀██▄    ██   ██   ██    ▀██▄                                    
  ██     ▀██    ██   ██   ██     ▀█████▄███  ▄██▀██▄▀████████▄   ▄▄█▀██ 
  ██      ██    ██   ██   ██      ██ ██▀ ▀▀ ██▀   ▀██ ██    ██  ▄█▀   ██
  ██     ▄██    ██   ██   ██     ▄██ ██     ██     ██ ██    ██  ██▀▀▀▀▀▀
  ██    ▄██▀██  ██   ██   ██    ▄██▀ ██     ██▄   ▄██ ██    ██  ██▄    ▄
▄████████▀  █████  ▄████▄████████▀ ▄████▄    ▀█████▀▄████  ████▄ ▀█████▀     

Fait par JANUSAUSKAS Tadas, AITOURAB Mehdi et Massengo Riche Hardy en classe de BTS SN2.

Ce fichier est le fichier principal d'exécution du projet.
 */

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DJI.WindowsSDK;
using Windows.UI.Popups;

namespace DJIDrone
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var module = navigationModules[0];
            NavView.MenuItems.Add(new NavigationViewItemHeader() { Content = module.header });
            foreach (var item in module.items)
            {
                NavView.MenuItems.Add(item.Key);
            }
        }

        private struct SDKModuleSampleItems
        {
            public String header;
            public List<KeyValuePair<String, Type>> items;
        }

        /// <summary>
        /// Liste de menus.
        /// </summary>
        private readonly List<SDKModuleSampleItems> navigationModules = new List<SDKModuleSampleItems>
        {
            new SDKModuleSampleItems() {
                header = "DJIDrone", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Activation de DJIDrone", typeof(DJISDKInitializing.PageActivation)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Vidéo", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Vidéo du drone", typeof(VideoDrone.PageVideo)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Composants", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Manipulation des composants", typeof(ComponentHandling.CHPage)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Points de passage", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Utilisation du simulateur", typeof(WaypointHandling.PageSimulateur)),
                    new KeyValuePair<string, Type>("Mission de points de passage", typeof(WaypointHandling.PagePoints)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Compte", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Gestion du compte", typeof(PageUtilisateur.PageUtilisateur)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Vol", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Zones de vol", typeof(Vol.PageVol)),
                },
            },
            new SDKModuleSampleItems() {
                header = "Lecture", items = new List<KeyValuePair<String, Type>>()
                {
                    new KeyValuePair<string, Type>("Lecture du drone", typeof(Playback.PlaybackPage)),
                },
            },

        };

        /// <summary>
        /// Chargement de la liste des options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            String invokedName = args.InvokedItem as String;
            foreach (var module in navigationModules)
            {
                foreach (var item in module.items)
                {
                    if (invokedName == item.Key)
                    {
                        if (ContentFrame.SourcePageType != item.Value)
                        {
                            ContentFrame.Navigate(item.Value);
                        }
                        return;
                    }
                }
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            DJISDKManager.Instance.SDKRegistrationStateChanged += Instance_SDKRegistrationEvent;
        }
        /// <summary>
        /// Active la navigation s'il n'y a pas d'erreur d'enregistrement du compte.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="resultCode"></param>
        private async void Instance_SDKRegistrationEvent(SDKRegistrationState state, SDKError resultCode)
        {
            if (resultCode == SDKError.NO_ERROR)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    for (int i = 1; i < navigationModules.Count; ++i)
                    {
                        var module = navigationModules[i];
                        NavView.MenuItems.Add(new NavigationViewItemHeader() { Content = module.header });
                        foreach (var item in module.items)
                        {
                            NavView.MenuItems.Add(item.Key);
                        }
                    }
                });
            }
            else
            {
                try
                {
                    MessageDialog message = new MessageDialog("Echec de la connexion.");
                    message.Commands.Add(new UICommand("OK"));
                    message.DefaultCommandIndex = 0;
                    message.CancelCommandIndex = 1;
                    await message.ShowAsync();
                }
                catch(Exception ex)
                {
                    MessageDialog message = new MessageDialog(ex.Message);
                    await message.ShowAsync();
                }
            }
        }
    }
}