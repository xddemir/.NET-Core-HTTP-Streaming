using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("comment_service_client");
    }

    public async Task<IActionResult> Index()
    {
        
        var comment = await _httpClient.GetFromJsonAsync<CommentViewModel>("api/Comment/stream/GetCommentStream");
        
        return View(comment);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}