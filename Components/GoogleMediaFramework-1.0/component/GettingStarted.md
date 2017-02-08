# Usage

##iOS
 
For full usage details on the entire library visit the [Google Media Framework iOS Wiki][iOSWiki]
 
The main class that we should be using is the `GMFPlayerViewController`, this is the `UIViewController` that contains both the content video player and the optional ad video player.

When you are ready to display your video, you can instantiate the `GMFPlayerViewController` and set its properties like the example below:
```csharp
            _videoPlayerViewController = new GMFPlayerViewController();

            _playbackDidFinishObserver = NSNotificationCenter.DefaultCenter.AddObserver(Constants.kGMFPlayerStateDidChangeToFinishedNotification,
                PlaybackDidFinish);

            _videoPlayerViewController.LoadStreamWithURL(NSUrl.FromString(video.VideoUrl));

            if (video.AdTagUrl != null)
            {
                _adService = new GMFIMAAdService(_videoPlayerViewController);
                _videoPlayerViewController.RegisterAdService(_adService);

                _adService.RequestAdsWithRequest(video.AdTagUrl);
            }

            ShowPlayer();
            _videoPlayerViewController.VideoTitle = video.Title;
            _videoPlayerViewController.Play();
```
			
You can refer to the [IMA SDK Reference][iOSIMA] for proper ad tag formats 

Once you have setup your `GMFPlayerViewController` you can add it as a Child `UIViewController`:

```csharp
            this.AddChildViewController(_videoPlayerViewController);
            _videoPlayerViewController.DidMoveToParentViewController(this);

            this.View.AddSubview(_videoPlayerViewController.View);
```

## Android

For full usage details on the entire library visit the [Google Media Framework Android Wiki][AndroidWiki]

The main class we are concerned with here is the `ImaPlayer`. This is the class that combines two `SimpleVideoPlayer` classes together, one for the content video and one for the ad content. 

Along with the video details and ad tag URL, the `ImaPlayer` also needs to be instantiated with the current `Activity` as well as a `FrameLayout` that will act as the container that the `ImaPlayer` will inject the video players and other UI elements.

Once you have the video information ready, you can prepare the `ImaPlayer` like so:

```csharp
            imaPlayer?.Release();

            videoPlayerContainer.RemoveAllViews();

            string adTagUrl = video.adUrl;
            string videoTitle = video.title;
            
            imaPlayer = new ImaPlayer(this, videoPlayerContainer, video.video, videoTitle, adTagUrl);
            imaPlayer.SetFullscreenCallback(this);

            imaPlayer.Play();
```
You can refer to the [IMA SDK Reference][AndroidIMA] for proper ad tag formats

Calling `Release()` on the `ImaPlayer` will ensure all views are destroyed and playback is halted, it is good practice to do this before starting a new video.

You may also want to have your `Activity` inherit from `PlaybackControlLayer.IFullscreenCallback` so you may set this `Activity` as the `ImaPlayer`'s FullscreenCallback to receive events when the user has toggled the Fullscreen button on the player.


###Thanks to

- [Martijn van Dijk][MartijnvanDijk] for the ExoPlayer Metadata fixes


[mit]: http://opensource.org/licenses/mit-license
[MartijnvanDijk]: https://github.com/martijn00
[AndGMF]: https://github.com/googleads/google-media-framework-android
[iOSGMF]: https://github.com/googleads/google-media-framework-ios
[IMASDK]:https://developers.google.com/interactive-media-ads/
[iOSIMA]:https://developers.google.com/interactive-media-ads/docs/sdks/ios/
[AndroidIMA]:https://developers.google.com/interactive-media-ads/docs/sdks/android/
[iOSWiki]:https://github.com/googleads/google-media-framework-ios/wiki
[AndroidWiki]:https://github.com/googleads/google-media-framework-android/wiki
[Exo]:http://developer.android.com/guide/topics/media/exoplayer.html