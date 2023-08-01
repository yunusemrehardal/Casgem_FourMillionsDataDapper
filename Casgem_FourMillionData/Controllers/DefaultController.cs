using Casgem_FourMillionData.DAL.DTOs;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Casgem_FourMillionData.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString = "Server=.;initial catalog = CARPLATES; integrated security= true";
        public async Task<IActionResult> Index()
        {
            await using var connection = new SqlConnection(_connectionString);

            await using var conneciton = new SqlConnection(_connectionString);

            var brandMax = (await conneciton.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await conneciton.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();

            var plateMax = (await conneciton.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await conneciton.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();

            var shiftType = (await conneciton.QueryAsync<ShiftTypeResult>("SELECT TOP 1 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count DESC")).FirstOrDefault();

            var fuelType = (await conneciton.QueryAsync<FuelResult>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();

            var colorType = (await conneciton.QueryAsync<ColorResult>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count DESC")).FirstOrDefault();



            ViewData["brandMax"] = brandMax.BRAND;
            ViewData["countMaxBrand"] = brandMax.Count;

            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countMinBrand"] = brandMin.Count;

            ViewData["plateMax"] = plateMax.PLATE;
            ViewData["countMaxPlate"] = plateMax.Count;

            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMinPlate"] = plateMin.Count;

            ViewData["shiftType"] = shiftType.SHIFTTYPE;
            ViewData["shiftTypeCount"] = shiftType.Count;

            ViewData["fuelType"] = fuelType.FUEL;
            ViewData["fuelTypeCount"] = fuelType.Count;

            ViewData["colorType"] = colorType.COLOR;
            ViewData["colorTypeCount"] = colorType.Count;

            return View();

        }

        public async Task<IActionResult> Search(string keyword)
        {

            string query = @"
            SELECT TOP 10000 BRAND, SUBSTRING(PLATE, 1, 2) AS PLATE, SHIFTTYPE, FUEL
            FROM PLATES
            WHERE BRAND LIKE '%' + @Keyword + '%'
               OR PLATE LIKE '%' + @Keyword + '%'
               OR SHIFTTYPE LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
        ";

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Sorguyu çalıştırın ve sonuçları alın
            var searchResults = await connection.QueryAsync<SearchResult>(query, new { Keyword = keyword });

            // Sonuçları JSON formatında döndürün
            return Json(searchResults);

        }
    }
}
