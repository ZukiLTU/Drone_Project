using DJI.WindowsSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DJIVideoParser;
using DJI.WindowsSDK.Components;
using Windows.UI.Popups;

namespace DJIDrone.VideoDrone
{
    public sealed partial class PageVideo : Page
    {
        /// <summary>
        /// Met le mode de la caméra
        /// </summary>
        public void SwitchMode()
        {
            DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).CameraWorkModeChanged += async delegate (object sender, CameraWorkModeMsg? val)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if(val != null)
                    {
                        lblMode.Text = "Mode : " + val.Value.value.ToString();
                    }
                });
            };
        }

        private async void SetCameraWorkMode(CameraWorkMode mode)
        {
            if (DJISDKManager.Instance.ComponentManager != null)
            {
                CameraWorkModeMsg workMode = new CameraWorkModeMsg
                {
                    value = mode,
                };
                var retCode = await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).SetCameraWorkModeAsync(workMode);
                if (retCode != SDKError.NO_ERROR)
                {
                             
                }
                else
                {
                    /*MessageDialog messageDialog = new MessageDialog("Erreur au niveau du SDK." + retCode.ToString());
                    await messageDialog.ShowAsync();*/
                }
            }
            else
            {
                //non enregistré
                Console.WriteLine("Licence non enregistrée.");
            }
        }
        private async void btnPrendrePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (DJISDKManager.Instance.ComponentManager != null)
            {
                var retCode = await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).StartShootPhotoAsync();
                if (retCode != SDKError.NO_ERROR)
                {
                    lblMsg.Text = "Echec de prise de photo : " + retCode.ToString();
                }
                else
                {
                    lblMsg.Text = "Photographié avec succès.";
                }
            }
            else
            {
                Console.WriteLine("L'application n'est pas enregistrée !");
            }
        }

        private async void btnDebutEnreg_Click(object sender, RoutedEventArgs e)
        {
            if (DJISDKManager.Instance.ComponentManager != null)
            {
                var retCode = await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).StartRecordAsync();
                if (retCode != SDKError.NO_ERROR)
                {
                    lblMsg.Text = "Ne peut pas enregistrer la vidéo : " + retCode.ToString();
                }
                else
                {
                    lblMsg.Text = "Enregistrement : ";
                    DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).RecordingTimeChanged += async delegate (object _sender, IntMsg? value)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            if (value != null)
                            {
                                lblMsg.Text = value.Value.value.ToString() + "s";
                            }
                        });
                    };
                }
            }
            else
            {
                lblMsg.Text = "L'application n'est pas enregistrée";
            }
        }

        private async void btnFinEnreg_Click(object sender, RoutedEventArgs e)
        {
            if (DJISDKManager.Instance.ComponentManager != null)
            {
                var retCode = await DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).StopRecordAsync();
                if (retCode != SDKError.NO_ERROR)
                {
                    lblMsg.Text = "Ne peut arrêter la vidéo  " + retCode.ToString();
                }
                else
                {
                    lblMsg.Text = "Fin de l'enregistrement";
                }
            }
            else
            {
                lblMsg.Text = "L'application n'est pas enregistrée.";
            }
        }

        private void btnmodePhoto_Click(object sender, RoutedEventArgs e)
        {
            SetCameraWorkMode(CameraWorkMode.SHOOT_PHOTO);
            lblMode.Text = "Mode photo.";
            lblMsg.Text = "";
        }

        private void btnmodeVideo_Click(object sender, RoutedEventArgs e)
        {
            SetCameraWorkMode(CameraWorkMode.RECORD_VIDEO);
            lblMode.Text = "Mode vidéo.";
            lblMsg.Text = "";
        }
    }
}