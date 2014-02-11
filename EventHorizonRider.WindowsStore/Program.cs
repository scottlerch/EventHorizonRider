using EventHorizonRider.Core;

namespace EventHorizonRider.WindowsStore
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var factory = new MonoGame.Framework.GameFrameworkViewSource<MainGame>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}
