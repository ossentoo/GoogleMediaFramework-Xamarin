GoogleMediaFramework-Xamarin
================

Xamarin Binding Library for GoogleMediaFramework (with IMA SDK integration).

This is the source code for the [Google Media Framework Xamarin Component][XamComp]

[Android GoogleMediaFramework][AndGMF]

[iOS GoogleMediaFramework][iOSGMF]

These bindings include the necessary files from the Android/iOS demo apps that use the [Google IMA SDK][IMASDK]

Thanks to
=========

- [Martijn van Dijk][MartijnvanDijk] for the ExoPlayer Metadata fixes

License
=======

- **GoogleMediaFramework-Xamarin** plugin is licensed under [MIT][mit]


[mit]: http://opensource.org/licenses/mit-license
[MartijnvanDijk]: https://github.com/martijn00
[AndGMF]: https://github.com/googleads/google-media-framework-android
[iOSGMF]: https://github.com/googleads/google-media-framework-ios
[IMASDK]:https://developers.google.com/interactive-media-ads/
[XamComp]:https://components.xamarin.com/view/googlemediaframework

Nuget Reinstall for Google Packages
===================================

When you first open the project, you may find that GMFSample references have a yellow triangle beside them.  To resolve this issue, issue the command below in the Visual Studio  Package Manager Console:

update-package -reinstall xamarin.googleplayservices.ads -version 29.0.0.1
