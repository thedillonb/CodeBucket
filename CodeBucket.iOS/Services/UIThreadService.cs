using System;
using CodeBucket.Core.Services;
using Foundation;

namespace CodeBucket.Services
{
	public class UIThreadService : IUIThreadService
    {
		private static readonly NSObject _obj = new NSObject();

		public void MarshalOnUIThread(Action a)
		{
			_obj.InvokeOnMainThread(() => 
            {
                try
                {
                    a();
                }
                catch (Exception e)
                { 
                    System.Diagnostics.Debug.WriteLine("Attempt to marshal on main thread ended in exception: " + e.Message);
                    System.Diagnostics.Debugger.Break();
                }
            });
		}
    }
}

