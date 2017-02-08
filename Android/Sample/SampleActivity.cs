using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Ads.Interactivemedia.V3.Api;
using Com.Google.Android.Libraries.Mediaframework.Exoplayerextensions;
using Com.Google.Android.Libraries.Mediaframework.Layeredvideo;
using Com.Google.Android.Libraries.Mediaframework.Players;

namespace GMFSample
{
    [Activity(Label = "SampleActivity", MainLauncher = true, Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden
)]

public class SampleActivity : Activity, PlaybackControlLayer.IFullscreenCallback
    {
        private ImaPlayer imaPlayer;
        private FrameLayout videoPlayerContainer;
        private ListView videoListView;
        private VideoListItem[] videoListItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            View view = LayoutInflater.Inflate(Resource.Layout.sample_activity, null);

            ActionBar?.Hide();

            videoPlayerContainer = (FrameLayout)view.FindViewById(Resource.Id.video_frame);
            videoListView = (ListView)view.FindViewById(Resource.Id.video_list_view);

            videoListItems = GetVideoListItems();
            string[] videoTitles = new string[videoListItems.Length];

            for (int i = 0; i < videoListItems.Length; i++)
            {
                videoTitles[i] = videoListItems[i].title;
            }

            videoListView.Adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleListItem1, videoTitles);

            videoListView.ItemClick += VideoListView_ItemClick;

            SetContentView(view);

        }

        private void VideoListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CreateImaPlayer(videoListItems[e.Position]);
        }

        protected override void OnDestroy()
        {
            imaPlayer?.Release();
            base.OnDestroy();
        }

        public void CreateImaPlayer(VideoListItem video)
        {
            imaPlayer?.Release();

            videoPlayerContainer.RemoveAllViews();

            string adTagUrl = video.adUrl;
            string videoTitle = video.title;

            var settings = new ImaSdkSettings {AutoPlayAdBreaks = true};
            imaPlayer = new ImaPlayer(this, videoPlayerContainer, video.video, videoTitle, adTagUrl);
            imaPlayer.SetFullscreenCallback(this);


            Drawable logo = Resources.GetDrawable(Resource.Drawable.gmf_icon);

            imaPlayer.SetLogoImage(logo);

            imaPlayer.Play();
        }

        public VideoListItem[] GetVideoListItems()
        {
            return new VideoListItem[] {
                new VideoListItem("No ads (DASH)",
                    new Video("http://www.youtube.com/api/manifest/dash/id/bf5bb2419360daf1/source/youtub" +
                        "e?as=fmp4_audio_clear,fmp4_sd_hd_clear&sparams=ip,ipbits,expire,as&ip=0.0.0.0&ip" +
                        "bits=0&expire=19000000000&signature=255F6B3C07C753C88708C07EA31B7A1A10703C8D.2D6" +
                        "A28B21F921D0B245CDCF36F7EB54A2B5ABFC2&key=ik0",
                        Video.VideoType.Dash,
                        "bf5bb2419360daf1"),
                    null),
                new VideoListItem("Skippable preroll (DASH)",
                    new Video("http://www.youtube.com/api/manifest/dash/id/bf5bb2419360daf1/source/youtub" +
                        "e?as=fmp4_audio_clear,fmp4_sd_hd_clear&sparams=ip,ipbits,expire,as&ip=0.0.0.0&ip" +
                        "bits=0&expire=19000000000&signature=255F6B3C07C753C88708C07EA31B7A1A10703C8D.2D6" +
                        "A28B21F921D0B245CDCF36F7EB54A2B5ABFC2&key=ik0",
                        Video.VideoType.Dash,
                        "bf5bb2419360daf1"),
                    "http://pubads.g.doubleclick.net/gampad/ads?sz=640x360&iu=/6062/iab_vast_samples/skippable&"
                    +"ciu_szs=300x250,728x90&impl=s&gdfp_req=1&env=vp&output=xml_vast2&unviewed_position_start=1&"
                        +"url=[referrer_url]&correlator=[timestamp]"),
                new VideoListItem("Unskippable preroll (DASH)",
                    new Video("http://www.youtube.com/api/manifest/dash/id/bf5bb2419360daf1/source/youtub" +
                        "e?as=fmp4_audio_clear,fmp4_sd_hd_clear&sparams=ip,ipbits,expire,as&ip=0.0.0.0&ip" +
                        "bits=0&expire=19000000000&signature=255F6B3C07C753C88708C07EA31B7A1A10703C8D.2D6" +
                        "A28B21F921D0B245CDCF36F7EB54A2B5ABFC2&key=ik0",
                        Video.VideoType.Dash,
                        "bf5bb2419360daf1"),
                    "http://pubads.g.doubleclick.net/gampad/ads?sz=400x300&iu=%2F6062%2Fhanna_MA_grou" +
                    "p%2Fvideo_comp_app&ciu_szs=&impl=s&gdfp_req=1&env=vp&output=xml_vast3&unviewed_p" +
                    "osition_start=1&m_ast=vast&url=[referrer_url]&correlator=[timestamp]"),
                new VideoListItem("Ad rules - 0s, 5s, 10s, 15s (DASH)",
                    new Video("http://www.youtube.com/api/manifest/dash/id/bf5bb2419360daf1/source/youtub" +
                        "e?as=fmp4_audio_clear,fmp4_sd_hd_clear&sparams=ip,ipbits,expire,as&ip=0.0.0.0&ip" +
                        "bits=0&expire=19000000000&signature=255F6B3C07C753C88708C07EA31B7A1A10703C8D.2D6" +
                        "A28B21F921D0B245CDCF36F7EB54A2B5ABFC2&key=ik0",
                        Video.VideoType.Dash,
                        "bf5bb2419360daf1"),
                    "http://pubads.g.doubleclick.net/gampad/ads?sz=400x300&iu=%2F6062%2Fgmf_demo&" +
                    "ciu_szs&impl=s&gdfp_req=1&env=vp&output=xml_vast3&unviewed_position_start=1&" +
                    "url=[referrer_url]&correlator=[timestamp]&ad_rule=1&cmsid=11924&vid=cWCkSYdF" +
                    "lU0&cust_params=gmf_format%3Dstd%2Cskip"),
                new VideoListItem("No ads (mp4)",
                    new Video("http://s0.2mdn.net/instream/videoplayer/media/android.mp4",
                        Video.VideoType.Mp4),
                    null),
                new VideoListItem("No ads - BBB (HLS)",
                    new Video("http://googleimadev-vh.akamaihd.net/i/big_buck_bunny/bbb-,480p,720p,1080p" +
                        ",.mov.csmil/master.m3u8",
                        Video.VideoType.Hls),
                    null),
                new VideoListItem("AdRules - Apple test (HLS)",
                    new Video("https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_4x3/" +
                        "bipbop_4x3_variant.m3u8 ",
                        Video.VideoType.Hls),
                    "http://pubads.g.doubleclick.net/gampad/ads?sz=400x300&iu=%2F6062%2Fgmf_demo&" +
                    "ciu_szs&impl=s&gdfp_req=1&env=vp&output=xml_vast3&unviewed_position_start=1&" +
                    "url=[referrer_url]&correlator=[timestamp]&ad_rule=1&cmsid=11924&vid=cWCkSYdF" +
                    "lU0&cust_params=gmf_format%3Dstd%2Cskip")
            };
        }
        public void OnGoToFullscreen()
        {
            videoListView.Visibility = ViewStates.Invisible;
        }

        public void OnReturnFromFullscreen()
        {
            videoListView.Visibility = ViewStates.Visible;
        }

        public class VideoListItem
        {
            public readonly string title;
            public readonly Video video;
            public readonly string adUrl;

            public VideoListItem(string title, Video video, string adUrl)
            {
                this.title = title;
                this.video = video;
                this.adUrl = adUrl;
            }
        }
    }
}