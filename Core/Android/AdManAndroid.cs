using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core;
using Microsoft.Xna.Framework;
using vec2 = Microsoft.Xna.Framework.Vector2;
namespace CloneTower
{

    public class AdManAndroid : AdMan
    {
        Dictionary<string, AdView> ads = new Dictionary<string, AdView>();//name to view id
        LinearLayout _adContainer;
        AndroidGameActivity _objActivity;

        public AdManAndroid(AndroidGameActivity act)
        {
            _objActivity = act;
            // The actual ad
            //https://freakingrectangle.wordpress.com/2017/02/18/first-blog-post/
            //https://www.nuget.org/packages/Xamarin.Firebase.Ads/
            _adContainer = new LinearLayout(_objActivity);
            _adContainer.Orientation = Orientation.Horizontal;
            _adContainer.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Top);
            _adContainer.SetBackgroundColor(Android.Graphics.Color.Transparent); // Need on some devices, not sure why
        }
        public void AddToLayout(FrameLayout layout)
        {
            layout.AddView(_adContainer);
        }
        public override void Add(string name, string unitId, vec2 xy, vec2 wh)
        {
            AdView adView = new AdView(_objActivity);
            adView.AdUnitId = unitId; // Get this id from admob "Monetize" tab
            adView.AdSize = AdSize.Banner;
            adView.Id = View.GenerateViewId();
            adView.LoadAd(new AdRequest.Builder()
                // .AddTestDevice("DEADBEEF9A2078B6AC72133BB7E6E177") // Prevents generating real impressions while testing
                .Build());

            ads.Add(name, adView);
        }
        public override void ShowAd(string adName)
        {
            AdView ad;
            if (ads.TryGetValue(adName, out ad))
            {
                if (_adContainer.FindViewById(ad.Id) == null)
                {
                    _adContainer.AddView(ad);
                }
            }
        }
        public override void HideAd(string adName)
        {
            AdView ad;
            if (ads.TryGetValue(adName, out ad))
            {
                if (_adContainer.FindViewById(ad.Id) != null)
                {
                    _adContainer.RemoveView(ad);
                }
            }

        }
        public override bool IsVisible(string adName) {
            AdView ad;
            if (ads.TryGetValue(adName, out ad))
            {
                return _adContainer.FindViewById(ad.Id) != null;
            }
            return false;
        }


    }
}