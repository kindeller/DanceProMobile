using System;
using System.Collections.Generic;
using DancePro.ViewModels;
using Foundation;
using DancePro.Services;

using UIKit;

namespace DancePro.iOS.ViewControllers
{
    public partial class MyMediaViewController : UICollectionViewController
    {
        FileTransferService service = new FileTransferService();
        private bool connecting = false;

        public MyMediaViewController(IntPtr intPtr) : base("MyMediaViewController", null)
        {

        }

        partial void OnConnectSwitchChanged(UISwitch sender)
        {
            if (sender.On)
            {
                Connect(sender);
            }
            else
            {
                Disconnect(sender);
            }
        }

        private void Disconnect(UISwitch sender)
        {
            service.CloseServer();
        }

        public void Connect(UISwitch sender)
        {

            List<UIAlertAction> actions = new List<UIAlertAction>();
            var action = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel,alert => { sender.On = false;
                connecting = false; });
            actions.Add(action);

            Alert("Connecting...", "Port: 827", actions);
            connecting = true;
            service.StartServer();

        }


        private void Alert(string title, string message, List<UIAlertAction> actions)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            foreach (var action in actions)
            {
                alert.AddAction(action);
            }
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            vc.PresentViewController(alert, true, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

