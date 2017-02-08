using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using GoogleMediaFramework;
using UIKit;

namespace iOSGMFSample
{


    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public GMFIMAAdService AdService { get; set; }
        public GMFPlayerViewController VideoPlayerViewController { get; set; }
        // class-level declarations
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var viewController = new VideoListViewController();

            var navController = new UINavigationController(viewController);

            navController.SetNavigationBarHidden(true, false);

            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = navController;
            // If you have defined a root view controller, set it here:
            // Window.RootViewController = myViewController;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}