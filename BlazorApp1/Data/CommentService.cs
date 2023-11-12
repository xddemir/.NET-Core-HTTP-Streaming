using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace BlazorApp1.Data;

public class CommentService
{

    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async IAsyncEnumerable<Comment> GetCommentsAsync(CancellationToken cancellationToken=default)
    {
        var commentResponse = await _httpClient.GetAsync("/api/Comment/stream/GetCommentStream", HttpCompletionOption.ResponseHeadersRead);

            var commentStream = await commentResponse.Content.ReadAsStreamAsync(cancellationToken);

            await foreach (var comment in 
                JsonSerializer.DeserializeAsyncEnumerable<Comment>(commentStream, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                    },
                    cancellationToken))
            {

                await Task.Delay(200);

                yield return new Comment()
                {
                    Body = comment.Body,
                    Email = comment.Email,
                    Id = comment.Id,
                    Name = comment.Name,
                    PostId = comment.PostId
                };
            }
    }
}