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

        public static AdSurface MainMenu => new AdSurface("main_menu");
        public static AdSurface Shop => new AdSurface("shop");
        public static AdSurface GameOver => new AdSurface("game_over");
        public static AdSurface LevelComplete => new AdSurface("level_complete");
        public static AdSurface Other => new AdSurface("other");

        public static AdSurface Custom(string customSurface) => new AdSurface(customSurface);

        public bool IsSet => _value != null;

        public static implicit operator string(AdSurface surface) => surface._value;
        public static implicit operator AdSurface(string value) => new AdSurface(value);

        public override string ToString() => _value ?? string.Empty;
    }
}