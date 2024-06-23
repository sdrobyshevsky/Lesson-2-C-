// Разработка web-приложения на C# (семинары)
// Урок 2. Работа с данными (CSV + статика), маппинг и кэширование
// Доработайте контроллер, реализовав в нем метод возврата CSV-файла с товарами.
// Доработайте контроллер, реализовав статичный файл со статистикой работы кэш. Сделайте его доступным по ссылке.
// Перенесите строку подключения для работы с базой данных в конфигурационный файл приложения.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;

        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            List<Product> products = GetProductsFromDatabase();

            // Конвертируем список товаров в CSV файл
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Id,Name,Price");
            foreach (var product in products)
            {
                csv.AppendLine($"{product.Id},{product.Name},{product.Price}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "products.csv");
        }

        private List<Product> GetProductsFromDatabase()
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=usersdb;Integrated Security=True";
        }

        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            // Чтение статичного файла с информацией о кэше
            string cacheStats = System.IO.File.ReadAllText("cache_statistics.txt");
            return Content(cacheStats, "text/plain");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}


// Разработка web-приложения на C# (семинары)
// Урок 2. Работа с данными (CSV + статика), маппинг и кэширование
// Доработайте контроллер, реализовав в нем метод возврата CSV-файла с товарами.
// Доработайте контроллер, реализовав статичный файл со статистикой работы кэш. Сделайте его доступным по ссылке.
// Перенесите строку подключения для работы с базой данных в конфигурационный файл приложения.

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IAppCache _cache;
        private readonly IConfiguration _configuration;

        public ProductsController(IAppCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            // Retrieve products from database
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Product> products = RetrieveProductsFromDatabase(connectionString);

            // Create csv string
            StringBuilder csvContent = new StringBuilder();
            foreach (var product in products)
            {
                csvContent.AppendLine($"{product.Id},{product.Name},{product.Price}");
            }

            // Return csv file
            byte[] data = Encoding.UTF8.GetBytes(csvContent.ToString());
            return File(data, "text/csv", "products.csv");
        }

        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            // Retrieve statistics from cache
            string statistics = _cache.Get<string>("Statistics");

            if (string.IsNullOrEmpty(statistics))
            {
                // If statistics not found in cache, generate it
                statistics = GenerateStatistics();
                _cache.Set("Statistics", statistics, TimeSpan.FromMinutes(30)); 
            }

            return Content(statistics, "text/plain");
        }

        private List<Product> RetrieveProductsFromDatabase(string connectionString)
        {
            "ConnectionStrings": {
            "DefaultConnection": "Server=(localdb)\\\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;"
            }

        private string GenerateStatistics()
        {
            string cacheStats = System.IO.File.ReadAllText("cache_statistics.txt");
            return Content(cacheStats, "text/plain");
        }
    }
}


// Разработка web-приложения на C# (семинары) 
// Урок 2. Работа с данными (CSV + статика), маппинг и кэширование 
// Доработайте контроллер, реализовав в нем метод возврата CSV-файла с товарами. 
// Доработайте контроллер, реализовав статичный файл со статистикой работы кэш. Сделайте его доступным по ссылке.
// Перенесите строку подключения для работы с базой данных в конфигурационный файл приложения.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
private readonly List _products = new List();

[HttpGet("csv")]
public async Task GetCsv()
{
StringBuilder sb = new StringBuilder();
sb.AppendLine("Id,Name,Price");

foreach (var product in _products)
{
sb.AppendLine($"{product.Id},{product.Name},{product.Price}");
}

byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
MemoryStream stream = new MemoryStream(byteArray);

return File(stream, "text/csv", "products.csv");
}

[HttpGet("statistic")]
public IActionResult GetStatic()
{
string staticContent = "Cache statistics: xyz"; // Замените на реальные данные о статистике кэша
byte[] byteArray = Encoding.UTF8.GetBytes(staticContent);
MemoryStream stream = new MemoryStream(byteArray);

return File(stream, "text/plain", "cache-statistic.txt");
}
}

public class Product
{
public int Id { get; set; }
public string Name { get; set; }
public decimal Price { get; set; }
}
}
```

// В данном коде были добавлены методы `GetCsv` для возврата CSV-файла с товарами и `GetStatic` для возврата статичного файла с статистикой кэша. 
// Также были добавлены соответствующие маршруты для доступа к этим методам.
// Чтобы перенести строку подключения к базе данных в конфигурационный файл, можно использовать файл `appsettings.json` следующим образом:

{
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True;"
}
}
```

// Затем в классе `Startup.cs` необходимо добавить чтение этой строки из конфигурации:

public void ConfigureServices(IServiceCollection services)
{
string connectionString = Configuration.GetConnectionString("DefaultConnection");

// Используйте connectionString для подключения к базе данных
}
```