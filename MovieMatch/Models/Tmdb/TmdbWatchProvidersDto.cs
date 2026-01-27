using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieMatch.Models.Tmdb
{
    public class TmdbWatchProvidersResponse
    {
        [JsonPropertyName("results")]
        public Dictionary<string, TmdbWatchProviderCountry> Results { get; set; } = new();
    }

    public class TmdbWatchProviderCountry
    {
        [JsonPropertyName("flatrate")]
        public List<TmdbWatchProvider> Flatrate { get; set; } = new();

    }

    public class TmdbWatchProvider
    {
        [JsonPropertyName("provider_id")]
        public int ProviderId { get; set; }

        [JsonPropertyName("provider_name")]
        public string ProviderName { get; set; } = string.Empty;

        [JsonPropertyName("logo_path")]
        public string LogoPath { get; set; } = string.Empty;
    }
}
