using MainWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MainWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await GetDepartmentsAsync();

            return View("DepartmentView",departments);
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
        public IActionResult Create()
        {
            Department department = new Department();
            return View( "CreateDepartment", department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await PostDepartmentAsync(department);

                if (isSuccess)
                {
                    return RedirectToAction("Index");  // Redirect to Index or success page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to insert department.");
                }
            }

            return View(department);
        }

        private async Task<bool> PostDepartmentAsync(Department department)
        {
            // Set base address and headers
            client.BaseAddress = new Uri("https://localhost:7209/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the department object to JSON
            var json = JsonSerializer.Serialize(department);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to the Web API
            HttpResponseMessage response = await client.PostAsync("api/Departments", content);

            // Return true if the request was successful
            return response.IsSuccessStatusCode;
        }
        private static async Task<List<Department>> GetDepartmentsAsync()
        {
            // Set the base address of the API
            client.BaseAddress = new Uri("https://localhost:7209/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Call the API
            HttpResponseMessage response = await client.GetAsync("api/Departments");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to a List<Department>
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Department>>(responseData, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve departments from API");
            }
        }
    }
}
