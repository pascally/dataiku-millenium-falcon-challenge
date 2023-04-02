using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MilleniumFalcon.Front.Pages;
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;


    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    [Required(ErrorMessage = "Empire config file is required")]
    public IFormFile EmpireConfig { get; set; }


    public string responseMessage { get; set; }

    public async Task OnPostAsync()
    {
        var filePath = Path.GetTempFileName();

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await EmpireConfig.CopyToAsync(fileStream);
        }

        var client = _httpClientFactory.CreateClient("WebAPI");
        var res = await client.PostAsync($"empire?path={filePath}", null);

        if (res.IsSuccessStatusCode)
        {
            var odds = await client.GetAsync("successodds");
            responseMessage = await odds.Content.ReadAsStringAsync();
        }
        else
        {
            responseMessage = res.ReasonPhrase;
        }

        ModelState.Clear();
    }
}
