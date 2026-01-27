using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieMatch.Models.Tmdb
{
    public class TmdbGenreList
    {
        [JsonPropertyName("genres")]
        public List<TmdbGenreDto> Genres { get; set; } = new();
    }

    public class TmdbGenreDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
