using EventHorizonRider.WindowsPhone8.Resources;

namespace EventHorizonRider.WindowsPhone8
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static readonly AppResources Resources = new AppResources();

        public AppResources LocalizedResources { get { return Resources; } }
    }
}