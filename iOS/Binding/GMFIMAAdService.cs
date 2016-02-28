using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;

namespace GoogleMediaFramework
{
    public class GMFIMAAdService : GMFAdService, IGMFPlayerOverlayViewControllerDelegate
    {
        private UIColor originalPlayPauseResetBackgroundColor;
        private GMFContentPlayhead contentPlayhead;

        private IMAAdsLoader adsLoader;
        private IMAAdsManager adsManager;
        private IMAAdDisplayContainer adDisplayContainer;

        private NSObject _willEnterForegroundObserver;

        private bool _hasVideoPlayerControl;

        public GMFIMAAdService(GMFPlayerViewController videoPlayerController) : base(videoPlayerController)
        {
            adsLoader = new IMAAdsLoader(CreateIMASettings());
            adsLoader.WeakDelegate = this;
            //RegisterAdsLoaderEvents();
        }

        public void RequestAdsWithRequest(string request)
        {

            var view = new UIView(VideoPlayerController.View.Bounds);
            VideoPlayerController.SetAboveRenderingView(view);
            adDisplayContainer = new IMAAdDisplayContainer(view, null);
            contentPlayhead = new GMFContentPlayhead(VideoPlayerController);




            IMAAdsRequest adsRequest = new IMAAdsRequest(request, adDisplayContainer, contentPlayhead, null);
            adsLoader.RequestAdsWithRequest(adsRequest);
        }

        /*	private void RegisterAdsLoaderEvents()
			{
				if (adsLoader != null)
				{
					UnregisterAdsLoaderEvents();
					adsLoader.FailedWithErrorData += OnAdsLoaderFailedWithErrorData;
					adsLoader.AdsLoadedWithData += OnAdsLoadedWithData;
				}
			}

			private void UnregisterAdsLoaderEvents()
			{
				adsLoader.FailedWithErrorData -= OnAdsLoaderFailedWithErrorData;
				adsLoader.AdsLoadedWithData -= OnAdsLoadedWithData;
			}
			*/

        private void RegisterAdsManagerEvents()
        {
            if (adsManager != null)
            {
                UnregisterAdsManagerEvents();

                _willEnterForegroundObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, ResumeAdOnForeground);

                adsManager.AdsManagerDidRequestContentPause += DidRequestContentPause;
                adsManager.AdsManagerDidRequestContentResume += DidRequestContentResume;
                adsManager.DidReceiveAdEvent += DidReceiveAdEvent;
                adsManager.DidReceiveAdError += DidReceiveAdError;
                adsManager.AdDidProgressToTime += AdDidProgressToTime;
            }
        }

        private void ResumeAdOnForeground(NSNotification n)
        {
            if (_hasVideoPlayerControl)
            {
                adsManager?.Resume();
            }
        }

        private void UnregisterAdsManagerEvents()
        {
            if (_willEnterForegroundObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willEnterForegroundObserver);
            }

            adsManager.AdsManagerDidRequestContentPause -= DidRequestContentPause;
            adsManager.AdsManagerDidRequestContentResume -= DidRequestContentResume;
            adsManager.DidReceiveAdEvent -= DidReceiveAdEvent;
            adsManager.DidReceiveAdError -= DidReceiveAdError;
            adsManager.AdDidProgressToTime -= AdDidProgressToTime;
        }

        private void DestroyAdsManager()
        {
            UnregisterAdsManagerEvents();
            adsManager?.Destroy();
        }

        private IMASettings CreateIMASettings()
        {
            var settings = new IMASettings();
            settings.Language = NSLocale.PreferredLanguages[0];
            settings.PlayerType = "google/gmf-ios";
            settings.PlayerVersion = "1.0.0";
            settings.EnableDebugMode = true;

            return settings;


        }

        [Export("adsLoader:adsLoadedWithData:")]
        public void AdsLoadedWithData(IMAAdsLoader adsLoader, IMAAdsLoadedData adsLoadedData)
        {
            adsManager = adsLoadedData.AdsManager;
            // GMFContentPlayhead handles listening for time updates from the video player and passing those
            // to the AdsManager.

            adsManager.InitializeWithAdsRenderingSettings(null);

            RegisterAdsManagerEvents();

            adsManager.Start();

        }

        [Export("adsLoader:failedWithErrorData:")]
        public void FailedWithErrorData(IMAAdsLoader adsLoader, IMAAdLoadingErrorData adErrorData)
        {
            System.Diagnostics.Debug.WriteLine($"Ad loading error {adErrorData.AdError.Message}");

            VideoPlayerController?.Play();
        }

        #region AdsLoaderDelegate
        /*	private void OnAdsLoaderFailedWithErrorData(object sender, AdFailedEventArgs e)
			{
				System.Diagnostics.Debug.WriteLine($"Ad loading error {e.AdErrorData.AdError.Message}");

				VideoPlayerController?.Play();
			}

			private void OnAdsLoadedWithData(object sender, AdLoadedEventArgs e)
			{
				// Get the ads manager from ads loaded data.
				adsManager = e.AdsLoadedData.AdsManager;


				// GMFContentPlayhead handles listening for time updates from the video player and passing those
				// to the AdsManager.
				contentPlayhead = new GMFContentPlayhead(VideoPlayerController);

				adsManager = new IMAAdsManager(contentPlayhead, null);

				RegisterAdsManagerEvents();

				adsManager.Start();
			}
			*/
        #endregion

        /*	[Export("adsManager:didReceiveAdEvent:")]
			void AdsManager(IMAAdsManager adsManager, IMAAdEvent adEvent)
			{
				VideoPlayerController.PlayerOverlayView.SetMediaTime(e.MediaTime);
			}

			[Export("adsManager:didReceiveAdError:")]
			void AdsManager(IMAAdsManager adsManager, IMAAdError error)
			{
				RelinquishControlToVideoPlayer();
				VideoPlayerController.Play();
			}

			[Export("adsManagerDidRequestContentPause:")]
			void AdsManagerDidRequestContentPause(IMAAdsManager adsManager)
			{

			}

			[Export("adsManagerDidRequestContentResume:")]
			void AdsManagerDidRequestContentResume(IMAAdsManager adsManager)
			{

			}

			[Export("adsManager:adDidProgressToTime:totalTime:"))]
			void AdsManager(IMAAdsManager adsManager, double mediaTime, double totalTime)
			{

			}

			[Export("adDidProgressToTime:totalTime:")]
			void AdDidProgressToTime(double mediaTime, double totalTime)
			{

			}

			[Export("adsManagerAdPlaybackReady:")]
			void AdsManagerAdPlaybackReady(IMAAdsManager adsManager)
			{

			}

			[Export("adsManagerAdDidStartBuffering:")]
			void AdsManagerAdDidStartBuffering(IMAAdsManager adsManager)
			{

			}

			[Export("adsManager:adDidBufferToMediaTime:mediaTime")]
			void AdsManager(IMAAdsManager adsManager, double mediaTime)
			{

			}*/

        private void AdDidProgressToTime(object sender, AdsManagerAdDidProgressToTimeEventArgs e)
        {
            VideoPlayerController.PlayerOverlayView.SetMediaTime(e.MediaTime);
        }

        private void DidReceiveAdError(object sender, AdsManagerErrorEventArgs e)
        {
            RelinquishControlToVideoPlayer();
            VideoPlayerController.Play();
        }

        private void DidReceiveAdEvent(object sender, AdsManagerAdEventEventArgs e)
        {
            var eventType = e.AdEvent.Type;
            System.Diagnostics.Debug.WriteLine($"** Ad Event **: {AdEventAsString(e.AdEvent.Type)}");

            switch (eventType)
            {
                case IMAAdEventType.Loaded:
                    VideoPlayerController.PlayerOverlayView.SetTotalTime(e.AdEvent.Ad.Duration);
                    VideoPlayerController.PlayerOverlayView.SetSeekbarTrackColor(UIColor.Yellow);
                    break;
                case IMAAdEventType.Started:
                case IMAAdEventType.Resume:
                    VideoPlayerController.PlayerOverlayView.DisableSeekbarInteraction();
                    VideoPlayerController.PlayerOverlayView.ShowPauseButton();
                    ShowPlayerControls();
                    break;
                case IMAAdEventType.Pause:
                    VideoPlayerController.PlayerOverlayView.ShowPlayButton();
                    ShowPlayerControls();
                    break;
                case IMAAdEventType.AllAdsCompleted:
                    RelinquishControlToVideoPlayer();
                    DestroyAdsManager();
                    //UnregisterAdsLoaderEvents();
                    break;
                default:
                    break;
            }
        }

        private void DidRequestContentResume(object sender, EventArgs e)
        {
            VideoPlayerController.SetControlsVisibility(visible: true, animated: true);
            RelinquishControlToVideoPlayer();
            VideoPlayerController.Play();
        }

        private void DidRequestContentPause(object sender, EventArgs e)
        {
            // IMA SDK wants control of the player, so pause and take over delegate from video controls.
            TakeControlOfVideoPlayer();
        }

        private void TakeControlOfVideoPlayer()
        {
            var playerViewController = VideoPlayerController;
            var overlayView = playerViewController.PlayerOverlayView;
            var overlayViewController =
                VideoPlayerController.VideoPlayerOverlayViewController;


            //TODO
            overlayViewController.IsAdDisplayed = true;
            overlayView.HideSpinner();

            originalPlayPauseResetBackgroundColor = overlayView.PlayPauseResetButtonBackgroundColor;

            overlayView.DisableTopBar();
            overlayView.PlayPauseResetButtonBackgroundColor = new UIColor(0, 0, 0, 0.5f);

            _hasVideoPlayerControl = true;
            VideoPlayerController.Pause();

            VideoPlayerController.SetVideoPlayerOverlayDelegate(new PlayerControlsViewDelegate(this));
        }

        private void ShowPlayerControls()
        {
            var overlayViewController =
                VideoPlayerController.VideoPlayerOverlayViewController;
            overlayViewController.ShowPlayerControlsAnimated(true);
        }

        private void RelinquishControlToVideoPlayer()
        {
            var playerViewController = VideoPlayerController;
            var overlayView = playerViewController.PlayerOverlayView;
            var overlayViewController =
                VideoPlayerController.VideoPlayerOverlayViewController;

            overlayViewController.IsAdDisplayed = false;
            overlayView.EnableSeekbarInteraction();
            overlayView.PlayPauseResetButtonBackgroundColor = UIColor.Black;

            VideoPlayerController.SetDefaultVideoPlayerOverlayDelegate();
            overlayView.SetSeekbarTrackColorDefault();

            overlayView.EnableTopBar();

            _hasVideoPlayerControl = false;

        }
        #region AdsManagerDelegate

        #endregion

        #region GMFAdService implementation

        #endregion

        #region GMFPlayerOverlayViewControllerDelegate
        public void DidPressPlay()
        {
            adsManager.Resume();
        }

        public void DidPressPause()
        {
            adsManager.Pause();
        }

        public void DidPressReplay()
        {
            // nothing to do
        }

        public void DidPressFullscreen(bool isFullscreen)
        {
            VideoPlayerController.DidPressFullscreen(isFullscreen);
        }

        //Cannot seek with ads
        public void DidSeekToTime(double time)
        {
            // nothing to do
        }

        public void DidStartScrubbing()
        {
            // nothing to do
        }

        public void DidEndScrubbing()
        {
            // nothing to do
        }
        #endregion

        public void PlayerControlsWillShow()
        {
            throw new NotImplementedException();
        }

        public void PlayerControlsDidShow()
        {
            throw new NotImplementedException();
        }

        public void PlayerControlsWillHide()
        {
            throw new NotImplementedException();
        }

        public void PlayerControlsDidHide()
        {
            throw new NotImplementedException();
        }

        #region Debug Methods

        private string AdEventAsString(IMAAdEventType eventType)
        {
            switch (eventType)
            {
                case IMAAdEventType.AllAdsCompleted:
                    return "All ads completed";
                case IMAAdEventType.Clicked:
                    return "Ad clicked";
                case IMAAdEventType.Complete:
                    return "Complete";
                case IMAAdEventType.FirstQuartile:
                    return "First quartile reached";
                case IMAAdEventType.Loaded:
                    return "Loaded";
                case IMAAdEventType.Midpoint:
                    return "Midpoint reached";
                case IMAAdEventType.Pause:
                    return "Ad paused";
                case IMAAdEventType.Resume:
                    return "Ad resumed";
                case IMAAdEventType.Tapped:
                    return "Ad tapped";
                case IMAAdEventType.ThirdQuartile:
                    return "Third quartile reached";
                case IMAAdEventType.Started:
                    return "Ad started";
                default:
                    return "Invalid event type";
            }
        }
        #endregion

        private class PlayerControlsViewDelegate : GMFPlayerControlsViewDelegate
        {
            private GMFIMAAdService _adService;

            public PlayerControlsViewDelegate(GMFIMAAdService adService)
            {
                _adService = adService;
            }

            [Export("didStartScrubbing")]
            public override void DidStartScrubbing()
            {
                _adService.DidStartScrubbing();
            }

            [Export("didEndScrubbing")]
            public override void DidEndScrubbing()
            {
                _adService.DidEndScrubbing();
            }

            [Export("didPressReplay")]
            public override void DidPressReplay()
            {
                _adService.DidPressPlay();
            }

            [Export("didSeekToTime:")]
            public override void DidSeekToTime(double time)
            {
                _adService.DidSeekToTime(time);
            }

            [Export("didPressFullscreen:")]
            public override void DidPressFullscreen(bool isFullscreen)
            {
                _adService.DidPressFullscreen(isFullscreen);
            }

            [Export("didPressPause")]
            public override void DidPressPause()
            {
                _adService.DidPressPause();
            }

            [Export("didPressPlay")]
            public override void DidPressPlay()
            {
                _adService.DidPressPlay();
            }
        }
    }
}

