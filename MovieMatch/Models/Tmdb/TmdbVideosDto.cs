using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieMatch.Models.Tmdb
{
    public class TmdbVideosResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbVideoDto> Results { get; set; } = new();
    }

    public class TmdbVideoDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("site")]
        public string Site { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("official")]
        public bool Official { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; } = string.Empty;
    }

    public class TmdbTrailerInfo
    {
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
