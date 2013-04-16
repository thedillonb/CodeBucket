using System;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("libOHAttributedLabel.a", LinkTarget.Simulator | LinkTarget.ArmV7 | LinkTarget.Thumb, ForceLoad = true, Frameworks = "CoreText")]