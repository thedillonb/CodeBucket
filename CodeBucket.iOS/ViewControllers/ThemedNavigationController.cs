using UIKit;

namespace CodeBucket.ViewControllers
{
    public class ThemedNavigationController : UINavigationController
    {
        public ThemedNavigationController(UIViewController ctrl)
            : base(ctrl)
        {
            ModalPresentationStyle = ctrl.ModalPresentationStyle;
            ModalTransitionStyle = ctrl.ModalTransitionStyle;
        }
    }
}