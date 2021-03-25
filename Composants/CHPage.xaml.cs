using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DJI.WindowsSDK.Components;
using Windows.UI.Popups;
using DJIDrone.ViewModels;

namespace DJIDrone.ComponentHandling
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class CHPage : Page
    {
        private FlightControllerHandler controllerHandler;
        public CHPage()
        {
            this.InitializeComponent();
            DataContext = new ComponentViewModel();
        }

        public async void btnDecollage_Click(object sender, RoutedEventArgs e)
        {
            try{
                if(controllerHandler != null) 
                {
                    controllerHandler.StartTakeoffAsync();
                }
            }
            catch(Exception ex)
            {
                MessageDialog message = new MessageDialog("Erreur : "+ ex.ToString());
                //await message.ShowAsync();
            }
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controllerHandler != null)
                {
                    controllerHandler.StartAutoLandingAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
