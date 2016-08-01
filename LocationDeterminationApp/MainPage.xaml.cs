using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System;
using Windows.UI.Xaml.Controls.Maps;
using System.Threading.Tasks;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace LocationDeterminationApp
{
    public sealed partial class MainPage : Page
    {
        private Geolocator _geolocator = null;
        private Geoposition _geoposition = null;

        private uint DESIRED_ACCURACY_IN_METERS = 5;
        private string DEFAULT_BUTTON_CONTENT = "Get my location!";
        private string GOT_BUTTON_CONTENT = "Got it!";
        private string DEFAULT_MAP_ICON_MESSAGE = "You are here!";

        public MainPage()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.ColorScheme = MapColorScheme.Light;//TODO check where it must be!
            MyMap.MapServiceToken = "Au3_aiNb3IkrBGkFtJ5VUh1F-kNkOU_pGvhqOpcN9k5y5uBHEbDa-Cgo_cBiOBO5";
            MyMap.LandmarksVisible = true;
            MyMap.TrafficFlowVisible = true;
        }

        private async Task<Geoposition> GetMyGeoPosition()
        {
            _geolocator = new Geolocator();
            _geolocator.DesiredAccuracyInMeters = DESIRED_ACCURACY_IN_METERS;

            try
            {
                _geoposition = await _geolocator.GetGeopositionAsync(
                           maximumAge: TimeSpan.FromSeconds(30),
                           timeout: TimeSpan.FromSeconds(5)
                          );
            }
            //If the location service is turned off then you can take the user to the location setting with the below code
            catch (Exception)
            {
                ContentDialog locationOffContentDialog = new ContentDialog();

                locationOffContentDialog.Content = "Hey, nothing to do here!\nIt seems like your " +
                    "location\nis turned OFF.\nPush OK to go to\nthe settings and\nturn ON your location.";

                locationOffContentDialog.PrimaryButtonText = "OK";
                locationOffContentDialog.SecondaryButtonText = "Cancel";

                var contentDialogResult = await locationOffContentDialog.ShowAsync();
                switch (contentDialogResult)
                {
                    //go to settings -> location to turn it ON if 'OK' button pressed
                    case ContentDialogResult.Primary:
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                        break;
                    //does nothing if 'Cancel' button pressed
                    case ContentDialogResult.Secondary:
                        break;
                    case ContentDialogResult.None:
                        break;
                }                
            }
            return _geoposition;
        }

        private async void GetLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationData.Text.Length.Equals(0))
            {
                Geoposition myCurrentGeoposition = await GetMyGeoPosition();

                var lattitude = myCurrentGeoposition.Coordinate.Latitude;
                var longitude = myCurrentGeoposition.Coordinate.Longitude;

                LocationData.Text = "Latt: " + lattitude.ToString("0.0000")
                        + ", Long: " + longitude.ToString("0.0000");

                GetLocationBtn.Content = GOT_BUTTON_CONTENT;

                var currentLocation = new Geopoint(
                    new BasicGeoposition() { Latitude = lattitude, Longitude = longitude });

                MyMap.Center = currentLocation;
                MyMap.ZoomLevel = 20;

                MapIcon myMapIcon = new MapIcon();
                myMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/AzureIcon.png"));
                myMapIcon.Location = currentLocation;
                myMapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.25, 0.9);
                myMapIcon.Title = DEFAULT_MAP_ICON_MESSAGE;
                myMapIcon.Visible = true;
                myMapIcon.ZIndex = int.MaxValue;
                MyMap.MapElements.Add(myMapIcon);
            }
            else
            {
                GetLocationBtn.Content = DEFAULT_BUTTON_CONTENT;
                LocationData.Text = string.Empty;
                MyMap.MapElements.Clear();                
            }
        }
    }
}