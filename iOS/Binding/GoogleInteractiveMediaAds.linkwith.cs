using System;
using ObjCRuntime;

[assembly: LinkWith("GoogleInteractiveMediaAds.a", LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator, SmartLink = true, ForceLoad = true, LinkerFlags = "-ObjC -w -lz", Frameworks = "AdSupport AudioToolbox SystemConfiguration CoreMedia WebKit MediaPlayer QuartzCore")]