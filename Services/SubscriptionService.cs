using System.Text.Json;
using System.Text;

namespace UnsoocialLandingPage.Services
{
	public class GoogleSheetService
	{
		private readonly HttpClient _httpClient;
		private readonly string _scriptUrl;
		private readonly ILogger<GoogleSheetService> _logger;

		public GoogleSheetService(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleSheetService> logger)
		{
			_httpClient = httpClient;
			_scriptUrl = configuration["GoogleSheetScriptUrl"];
			_logger = logger;
		}

		public async Task<string> AddSubscriptionAsync(string email)
		{
			if (string.IsNullOrEmpty(_scriptUrl) || string.IsNullOrEmpty(email))
			{
				_logger.LogError("Script URL or Email is null or empty.");
				return "error";
			}

			// Create a JSON payload to send in the request body
			var payload = new { email = email };
			var jsonPayload = JsonSerializer.Serialize(payload);
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			try
			{
				_logger.LogInformation("POSTing to Google Sheet script at URL: {URL}", _scriptUrl);

				// Use PostAsync instead of GetAsync
				var response = await _httpClient.PostAsync(_scriptUrl, content).ConfigureAwait(false);

				var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				_logger.LogInformation("Received response body from Google: {ResponseBody}", responseBody);

				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError("Google Script returned a non-success status code: {StatusCode}", response.StatusCode);
					return "error";
				}

				try
				{
					var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
					return result.GetProperty("result").GetString() ?? "error";
				}
				catch (JsonException jsonEx)
				{
					_logger.LogError(jsonEx, "Failed to deserialize JSON response. The response may not be valid JSON.");
					return "error";
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP request to Google Sheet script failed.");
				return "error";
			}
		}
	}
}
