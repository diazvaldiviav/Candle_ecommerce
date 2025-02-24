namespace Candle_API.CoreSettings
{
    // Configuration/CorsSettings.cs
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; }
        public string[] AllowedMethods { get; set; }
        public string[] AllowedHeaders { get; set; }
        public string[] ExposedHeaders { get; set; }
        public int MaxAge { get; set; }
    }
}
