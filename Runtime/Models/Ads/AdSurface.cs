namespace BoltApp
{
    /// <summary>
    /// Type-safe wrapper for ad surface names. Used in ad analytic reporting.
    /// </summary>
    public readonly struct AdSurface
    {
        private readonly string _value;

        private AdSurface(string value)
        {
            _value = value;
        }

        public static AdSurface MainMenu => new AdSurface("MainMenu");
        public static AdSurface Shop => new AdSurface("Shop");
        public static AdSurface LevelComplete => new AdSurface("LevelComplete");
        public static AdSurface GameOver => new AdSurface("GameOver");
        public static AdSurface Other => new AdSurface("Other");

        public static AdSurface Custom(string customSurface) => new AdSurface(customSurface);

        public bool IsSet => _value != null;

        public static implicit operator string(AdSurface surface) => surface._value;

        public override string ToString() => _value ?? string.Empty;
    }
}