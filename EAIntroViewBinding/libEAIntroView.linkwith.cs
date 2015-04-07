using System;
using ObjCRuntime;

[assembly: LinkWith("libEAIntroView.a", LinkTarget.ArmV7 | LinkTarget.Simulator | LinkTarget.Arm64 | LinkTarget.ArmV7s | LinkTarget.Simulator64, SmartLink = true, ForceLoad = true)]
