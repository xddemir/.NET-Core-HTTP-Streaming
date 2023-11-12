using System.Diagnostics;
using System.Runtime.CompilerServices;
using HttpStreaming.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HttpStreaming.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public CommentController(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("posts_service_client");
    }
    
    [HttpGet]
    [Route("task/GetCommentTask")]
    public async Task<List<Comment>>GetCommentTask()
    {
        var comments = new List<Comment>();
        var tasks = new List<Task<Comment>>();
        
        for (int i = 1; i < 500; i++)
        {
            
            string requestEndpoint = $"comments/{i}";
            tasks.Add(FetchCommentAsync(requestEndpoint, CancellationToken.None));
        }
        
        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);

            comments.Add(await completedTask);
        }
        
        return comments;
    }
    
    [HttpGet]
    [Route("taskstream/GetCommentTask")]
    public async IAsyncEnumerable<Comment>GetCommentTask([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<Comment>>();
        
        for (int i = 1; i < 500; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            string requestEndpoint = $"comments/{i}";
            tasks.Add(FetchCommentAsync(requestEndpoint, cancellationToken));

        }
        
        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);

            yield return await completedTask;
        }
    }
    
    private async Task<Comment> FetchCommentAsync(string requestEndpoint, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(requestEndpoint, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Comment>();
        }

        return null;
    }

    [HttpGet]
    [Route("stream/GetCommentStream")]
    public async Task<IEnumerable<Comment>> GetCommentStream([EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"comments");
        return await response.Content.ReadFromJsonAsync<IEnumerable<Comment>>(cancellationToken: cancellationToken);
    }
    
    [HttpGet]
    [Route("GetComment")]
    public async Task<List<Comment>> GetComment()
    {
        var comments = new List<Comment>();
        
        for (int i = 1; i < 500; i++)
        {
            string requestEndpoint = $"comments/{i}";
            comments.Add(await _httpClient.GetFromJsonAsync<Comment>(requestEndpoint, CancellationToken.None));
        }
    
        return comments;
    }
    
}