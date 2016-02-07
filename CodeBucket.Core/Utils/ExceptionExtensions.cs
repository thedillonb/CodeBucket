using MvvmCross.Platform;

using CodeBucket.Core.Services;


namespace System
{
	public static class ExceptionExtensions
    {
		public static void Report(this Exception e)
		{

		}

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Dump(this Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message + " - " + e.StackTrace);
        }
    }
}

