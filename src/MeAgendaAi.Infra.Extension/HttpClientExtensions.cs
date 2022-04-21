
using System.Net.Http.Json;
using System.Text.Json;

namespace MeAgendaAi.Infra.Extension;

public static class HttpClientJsonExtensions
{
	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(
		this HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
	{
		if (client is null)
			throw new ArgumentNullException(nameof(client));

		var content = JsonContent.Create(value, mediaType: null, options);
		return client.PatchAsync(requestUri, content, cancellationToken);
	}
}