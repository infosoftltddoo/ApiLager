using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LagerController : ControllerBase
{
    private readonly IConfiguration _config;
    public LagerController(IConfiguration config) => _config = config;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { status = "error", message = "Query 'q' je obavezan" });

        const string sql = @"
            SELECT
              s.Artikal AS sifra,
              a.Naziv   AS naziv,
              a.Cena    AS cena,
              SUM(v.Znak * s.Kolicina) AS skol
            FROM dbo.Stavke s
            JOIN firme.dbo.[Vrste dokumenta] v ON s.vrdok = v.Vrsta
            JOIN dbo.Artikli a ON s.Artikal = a.Sifra AND a.Magacin = s.Magacin
            WHERE s.Magacin = 1
              AND a.Naziv LIKE @search
            GROUP BY s.Artikal, a.Naziv, a.Cena
            HAVING SUM(v.Znak * s.Kolicina) > 0;
        ";

        try
        {
            var results = new List<object>();
            await using var conn = new SqlConnection(_config.GetConnectionString("Agsol2025"));
            await using var cmd = new SqlCommand(sql, conn);

            // Precizno postavljanje tipa i dužine parametra
            cmd.Parameters.Add("@search", SqlDbType.VarChar, 200)
               .Value = $"%{q}%";

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    sifra = reader["sifra"].ToString(),
                    naziv = reader["naziv"].ToString(),
                    cena = Convert.ToDecimal(reader["cena"]),
                    skol = Convert.ToDecimal(reader["skol"])
                });
            }

            return Ok(new { status = "success", query = q, results });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { status = "error", message = "DB error", details = ex.Message });
        }
    }
}

