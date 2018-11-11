using Core;
using Microsoft.Xna.Framework;
using System;
using System.IO.IsolatedStorage;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Windows10UWP
{

    public class UWPGameSystem : GameSystem
    {
        public enum DeviceFormFactorType
        {
            Phone,
            Desktop,
            Tablet,
            IoT,
            SurfaceHub,
            Other
        }
        public static DeviceFormFactorType GetDeviceFormFactorType()
        {
            //https://gist.github.com/wagonli/40d8a31bd0d6f0dd7a5d
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFormFactorType.Phone;
                case "Windows.Desktop":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse
                        ? DeviceFormFactorType.Desktop
                        : DeviceFormFactorType.Tablet;
                case "Windows.Universal":
                    return DeviceFormFactorType.IoT;
                case "Windows.Team":
                    return DeviceFormFactorType.SurfaceHub;
                default:
                    return DeviceFormFactorType.Other;
            }
        }

        public UWPGameSystem(GameBase g) : base(g) { }
        public override void Exit()
        {
            Game.Exit();
        }
        public override bool LoadData(string filename, out string data)
        {
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types
            //Calling wait, causeas a deadlock, see
            //https://stackoverflow.com/questions/17248680/await-works-but-calling-task-result-hangs-deadlocks
            data = Task.Run(() => LoadData2(filename)).Result;
            return true;
        }
        public override bool SaveData(string filename, string data)
        {
            //Calling wait, causeas a deadlock, see
            //https://stackoverflow.com/questions/17248680/await-works-but-calling-task-result-hangs-deadlocks
            Task.Run(() => SaveData2(filename, data));
            
            return true;
        }
        public async void SaveData2(string filename, string data)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile f = await storageFolder.CreateFileAsync(filename,
                    CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(f, data);
        }
        public async Task<string> LoadData2(string filename)
        {
            string data = "";

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile f = null;
            try
            {
                f = await storageFolder.GetFileAsync(filename);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Not found
            }
            if (f == null)
            {
                SaveData2(filename, "");
            }
            else
            {
                data = await FileIO.ReadTextAsync(f);
            }

            return data;
        }
        public override void HideNav()
        {

        }

        public override Platform GetPlatform()
        {
            if(GetDeviceFormFactorType() == DeviceFormFactorType.Desktop)
            {
                return Platform.Desktop;
            }
            else if (GetDeviceFormFactorType() == DeviceFormFactorType.Phone)
            {
                return Platform.WindowsPhone;
            }
            return Platform.Desktop;
        }
    }
    public class UWPAdMan : AdMan
    {
        Microsoft.Advertising.WinRT.UI.AdControl _objAd;
        public UWPAdMan(Microsoft.Advertising.WinRT.UI.AdControl ad)
        {
            _objAd = ad;
        }
        public override void Add(string name, string unitId, Vector2 xy, Vector2 wh)
        {
        }

        public override void HideAd(string adName)
        {
            _objAd.Visibility = Visibility.Collapsed;
        }

        public override bool IsVisible(string adName)
        {
            return _objAd.Visibility == Visibility.Visible;
        }

        public override void ShowAd(string adName)
        {
            _objAd.Visibility = Visibility.Visible;
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly MainGame _game;

		public GamePage()
        {
            InitializeComponent();

            //Windows Store UWP
            //Dev license - $20 ($99 corporate, later)
            //**RESERVE A APP NAME FIRST**

            //UWP Ads
            //https://docs.microsoft.com/en-us/windows/uwp/monetize/adcontrol-in-xaml-and--net
            //Install Microsoft.Avertisign.XAML from nuget
            //https://docs.microsoft.com/en-us/windows/uwp/monetize/install-the-microsoft-advertising-libraries#install-nuget
            //Eror where not found https://social.msdn.microsoft.com/Forums/windowshardware/en-US/24f18438-0e19-4917-90c2-7b3d745f355b/uwpc-the-name-adcontrol-does-not-exist-in-the-namespace?forum=aiasdk
            //**NOTE AFTER NUGET PACKAGE YOU MUST ADD MICROSOFT ADVERTISING SDK FOR XAML REFERENCE (Reference -> add ->UWP ->Extensions ->>>)!!!
            //***TEST***
            // **BANNER ONLY **
            //            ApplicationId="3f83fe91-d6be-434d-a0ae-7351c5a997f1"
            //      AdUnitId = "test"
            //**PROD***
            // App ID = 9mwk7g6t5m0w
            //AdUnitId = 1100022107
            // 

            //https://docs.microsoft.com/en-us/windows/uwp/monetize/set-up-ad-units-in-your-app#live-ad-units
            /*
             * Note
                The application ID values for test ad units and live UWP ad units have different formats. Test application ID values are GUIDs.
                When you create a live UWP ad unit in the dashboard, the application ID value for the ad unit always matches the Store ID for your
                app (an example Store ID value looks like 9NBLGGH4R315).
                             * 
             */
            //**Follow this guide to package
            //https://docs.microsoft.com/en-us/windows/uwp/packaging/packaging-uwp-apps

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<MainGame>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);

            //Begin Visible
            MainAd.Visibility = Visibility.Visible;

           // swapChainPanel.Visibility = Visibility.Collapsed;
//            MainAd.SetValue(Canvas.ZIndexProperty, )
            _game.Init(new UWPAdMan(MainAd), true, new UWPGameSystem(_game));
        }
    }
}
