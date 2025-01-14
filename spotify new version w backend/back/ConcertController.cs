using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting.Server;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Unicode;
using System;

namespace spotify_concert_app_backend
{
    [ApiController]
    [Route("api")]
    public class ConcertController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _ticketmasterApiKey;
        private readonly string _spotifyClientId;
        private readonly string _spotifyClientSecret;

        public ConcertController(IConfiguration config)

        {
            _httpClient = new HttpClient();
            _ticketmasterApiKey = config["Ticketmaster:ApiKey"];
            _spotifyClientId = config["Spotify:ClientId"];
            _spotifyClientSecret = config["Spotify:ClientSecret"];
        }

        [HttpGet("concerts")]
        public async Task<IActionResult> GetConcerts(string artist)
        {
            if (string.IsNullOrEmpty(_ticketmasterApiKey))
                return BadRequest("Brak klucza API do Ticketmaster");

            var url = $"https://app.ticketmaster.com/discovery/v2/events.json?keyword={artist}&apikey={_ticketmasterApiKey}";
            var response = await _httpClient.GetStringAsync(url);

            return Ok(JsonSerializer.Deserialize<object>(response));
        }

        [HttpGet("spotify-token")]
        public async Task<IActionResult> GetSpotifyToken()
        {
            if (string.IsNullOrEmpty(_spotifyClientId) || string.IsNullOrEmpty(_spotifyClientSecret))
                return BadRequest("Brak danych do Spotify");

            var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_spotifyClientId}:{_spotifyClientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Add("Authorization", $"Basic {auth}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<JsonElement>(content).GetProperty("access_token").GetString();

            return Ok(new { AccessToken = token });
        }

        [HttpPost("calendar-event")]
        public async Task<IActionResult> AddToCalendar([FromBody] object eventData)
        {
            var authToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
     
            if (string.IsNullOrEmpty(authToken))
                return Unauthorized("Brak tokenu autoryzacyjnego");

            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/calendar/v3/calendars/primary/events");
            request.Headers.Add("Authorization", $"Bearer {authToken}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(eventData),
                System.Text.Encoding.UTF8,
                "application/json"  
            );

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return Ok(JsonSerializer.Deserialize<object>(content));
        }
    }
}