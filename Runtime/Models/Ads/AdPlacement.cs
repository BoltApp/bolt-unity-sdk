namespace BoltApp
{
    /// <summary>
    /// Type-safe wrapper for ad placement names. Used in ad analytic reporting.
    /// </summary>
    public readonly struct AdPlacement
    {
        private readonly string _value;

        private AdPlacement(string value)
        {
            _value = value;
        }

        public static AdPlacement MainMenu => new AdPlacement("MainMenu");
        public static AdPlacement Shop => new AdPlacement("Shop");
        public static AdPlacement LevelComplete => new AdPlacement("LevelComplete");
        public static AdPlacement GameOver => new AdPlacement("GameOver");
        public static AdPlacement Other => new AdPlacement("Other");

        public static AdPlacement Custom(string customPlacement) => new AdPlacement(customPlacement);

        public static implicit operator string(AdPlacement placement) => placement._value;

        public override string ToString() => _value ?? string.Empty;
    }
}