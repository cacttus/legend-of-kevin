using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using Core;
using System;
using System.IO.IsolatedStorage;
using vec2 = Microsoft.Xna.Framework.Vector2;

namespace CloneTower
{
    public class AndroidGameSystem : GameSystem
    {
        public override Platform GetPlatform()
        {
            return Core.Platform.Android;
        }

        Microsoft.Xna.Framework.AndroidGameActivity activity;
        public AndroidGameSystem(GameBase g, Microsoft.Xna.Framework.AndroidGameActivity act) : 
            base(g)
        {
            activity = act;
            
        }
        public override void Exit()
        {
            Game.Exit();
        }
        public override bool SaveData(string filename, string data)
        {
            var prefs = activity.GetSharedPreferences(activity.PackageName, FileCreationMode.Private);

            var prefEditor = prefs.Edit();

            prefEditor.PutString(filename, data);

            prefEditor.Commit();

            return true;
        }
        public override bool LoadData(string filename, out string data)
        {
            var prefs = activity.GetSharedPreferences(activity.PackageName, FileCreationMode.Private);

            data = prefs.GetString(filename, "");

            return true;
        }
        public override void HideNav()
        {
            int uiOptions = (int)activity.Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }
    }

    [Activity(Label = "Pocket Miner"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {


            base.OnCreate(bundle);


            //https://stackoverflow.com/questions/39248138/how-to-hide-bottom-bar-of-android-back-home-in-xamarin-forms
            //This actually prevents it from showing COMPLETELY


            var mainLayout = new FrameLayout(this);
            var ads = new AdManAndroid(this);
            var g = new MainGame();
            g.Init(ads, true, new AndroidGameSystem(g, this));

            //God firebase is confusing.  Go here.
            //https://apps.admob.com/v2/home?pli=1
            //Test ad ca-app-pub-3940256099942544/6300978111
            //APP ID ca-app-pub-4691006381926044~5783701266
            //Prod. Ad ca-app-pub-4691006381926044/9695026110
            //
            //**Firebase -> Settings -> "Add to Android" button
            //
            //To get - Google Services Json - 
            //      Sign in to Firebase and open your project.
            //            Click and select Project settings.
            //          In the Your apps card, select the package name of the app you need a config file for from the list.
            //          Click google - services.json.
            //*To deploy
            //      *Set Release
            //       Rigth click android -> Archive Manager
            //       Archive, then click "Deploy" on that screen.
            //       Ad hoc - just create the APK (that's all you need really)
            //        Load to GPDC
            //**LINK TO FIREBASE**
            //  IN ADMOB -> SETTINGS -> LINK TO FIREBASE
            //  **THIS MUST BE DONE IN ORDER FOR AD TO SHOW**
            //  **THIS MUST BE DONE IN ORDER FOR AD TO SHOW**
            //                     ca-app-pub-4691006381926044/9695026110
            //
            // AMAZON: https://developer.amazon.com/beta/M17FH17FBG8NC4/create.html
            //  Armor Monkey Amazon ACCOUNT
            //  dpage88 @gmail.com
            //  ^Armor2018*
             ads.Add("MainAd", "ca-app-pub-4691006381926044/8749814339", new vec2(0, 0), new vec2(100, 100));

            mainLayout.AddView((View)g.Services.GetService(typeof(View)));


            ads.AddToLayout(mainLayout);
            SetContentView(mainLayout);

            g.Run();
            
        }

    }
}





/*
 * English speaking coutnries for Adwords
American Samoa
Anguilla
Antigua
Australia
Bahamas
Barbados
Barbuda
Belau
Belize
Bermuda
Botswana
British Virgin Islands
Cameroon
Canada
Cayman Islands
Christmas Island
Cocos (Keeling) Islands
Cook Islands
Dominica
Falkland Islands
Fiji
Gambia
Ghana
Gibraltar
Grenada
Guam
Guernsey
Guyana
Hong Kong
India
Ireland
Isle of Man
Jamaica
Jersey
Johnston
Jordan
Kenya
Kiribati
Lesotho
Liberia
Malawi
Malaysia
Malta
Marshall Islands
Mauritius
Micronesia
Montserrat
Namibia
Nauru
New Zealand
Nigeria
Niue
Norfolk Island
Northern Mariana Islands
Pakistan
Palau
Papua New Guinea
Philippines
Picairn Islands
Puerto Rico
Rwanda
Samoa
Seychelles
Sierra Leone
Singapore
Sint Maarten
Solomon Islands
South Africa
South Korea
South Sudan
Sri Lanka
St, Lucia
St, Vincent & the Grenadines
St. Helena
St. Kitts & Nevis
Sudan
Swaziland
Tanzania
Tokelau
Tonga
Trinidad & Tobago
Turks & Caicos Islands
Tuvalu
Uganda
UK
US
US Virgin Islands
Vanuatu
Zambia
Zimbabwe 
*/
