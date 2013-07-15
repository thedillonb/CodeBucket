using System;
using MonoTouch.UIKit;

namespace CodeBucket.Utils
{
    /// <summary>
    /// Just some silly transition code that I didn't want to write a hundred times over and over again.
    /// </summary>
    public static class Transitions
    {
        public static void TransitionToController(UIViewController controller)
        {
            Transition(controller, UIViewAnimationOptions.TransitionCrossDissolve, 1.0);
        }

        public static void Transition(UIViewController controller, UIViewAnimationOptions options, double duration = 0.6)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            UIView.Transition(window, duration, options, () => {
                var oldState = UIView.AnimationsEnabled;
                UIView.AnimationsEnabled = false;
                window.RootViewController = controller;
                UIView.AnimationsEnabled = oldState;
            }, null);
        }
    }
}

