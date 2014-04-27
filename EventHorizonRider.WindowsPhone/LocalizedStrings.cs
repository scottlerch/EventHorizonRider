using EventHorizonRider.WindowsPhone.Resources;

namespace EventHorizonRider.WindowsPhone
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