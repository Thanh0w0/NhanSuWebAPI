using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models.ViewModels;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/ThongKes/ThongKe").Result;

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response asynchronously
                var result = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ThongKeHomeVM>(result);

                // Pass the data to the view
                return View(data);
            }

            // Handle failure (you can show a custom error message)
            return View("Error");
        }
    }
}
