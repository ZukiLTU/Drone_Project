using DJI.WindowsSDK;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YamlDotNet.RepresentationModel;

namespace DJIDrone.DJISDKInitializing
{
    public sealed partial class PageActivation: Page
    {
        bool auth;
        /// <summary>
        /// Enregistrement de l'utilisteur
        /// </summary>
        public PageActivation()
        {
            this.InitializeComponent();
            DJISDKManager.Instance.SDKRegistrationStateChanged += Instance_SDKRegistrationEvent;
        }

        private async void Instance_SDKRegistrationEvent(SDKRegistrationState state, SDKError resultCode)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                activateStateTextBlock.Text = state == SDKRegistrationState.Succeeded ? "Activé." : "Inactif.";
                activationInformation.Text = resultCode == SDKError.NO_ERROR ? "Enregistré avec succès." : resultCode.ToString();
            if (resultCode == SDKError.NO_ERROR)
                {
                    auth = true;
                    switch (auth) 
                    {
                        case true:
                            break;
                    }
                }
            });
        }
         
        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            async void GetYML(string chemin) 
            {
                try
                {
                    using (var reader = new StreamReader(chemin))
                    {
                        var yml = new YamlStream();
                        yml.Load(reader);

                        var mapping = (YamlMappingNode)yml.Documents[0].RootNode;
                        var items = mapping.Children[new YamlScalarNode("api_key")];
                        lblCode.Text = items.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog message = new MessageDialog(ex.Message);
                    await message.ShowAsync();
                }
                finally
                {
                    DJISDKManager.Instance.RegisterApp(lblCode.Text);
                    activationInformation.Text = "Enregistrement...";
                }
            }
            GetYML(AppDomain.CurrentDomain.BaseDirectory + "config.yml");       
        }
    }
}
