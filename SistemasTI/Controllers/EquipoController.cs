using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly string? _connectionString;

    public EquipoController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    [HttpGet]
    public IActionResult GetEquipos()
    {
        List<object> equipos = new List<object>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = "SELECT Id, Tipo, Marca, Modelo, NumeroSerie, Estado, FechaCompra, Garantia, Notas FROM Equipos";
            SqlCommand cmd = new SqlCommand(query, conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    equipos.Add(new
                    {
                        id = reader["Id"],
                        tipo = reader["Tipo"]?.ToString(),
                        marca = reader["Marca"]?.ToString(),
                        modelo = reader["Modelo"]?.ToString(),
                        serie = reader["NumeroSerie"]?.ToString(),
                        estado = reader["Estado"]?.ToString(),
                        fechaCompra = reader["FechaCompra"] != DBNull.Value ?
    Convert.ToDateTime(reader["FechaCompra"]).ToString("yyyy-MM-dd") : "",
                        garantia = reader["Garantia"] != DBNull.Value ?
    Convert.ToDateTime(reader["Garantia"]).ToString("yyyy-MM-dd") : "",
                        notas = reader["Notas"]?.ToString()
                    });
                }
            }
        }
        return Ok(equipos);
    }

    [HttpPost]
    public IActionResult CrearEquipo([FromBody] EquipoRequest request)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = @"INSERT INTO Equipos (Tipo, Marca, Modelo, NumeroSerie, Estado, FechaCompra, Garantia, Notas)
                            VALUES (@tipo, @marca, @modelo, @serie, @estado, @fechaCompra, @garantia, @notas)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tipo", request.Tipo);
            cmd.Parameters.AddWithValue("@marca", request.Marca);
            cmd.Parameters.AddWithValue("@modelo", request.Modelo);
            cmd.Parameters.AddWithValue("@serie", request.Serie);
            cmd.Parameters.AddWithValue("@estado", request.Estado);
            cmd.Parameters.AddWithValue("@fechaCompra", request.FechaCompra);
            cmd.Parameters.AddWithValue("@garantia", string.IsNullOrEmpty(request.Garantia) ? DBNull.Value : request.Garantia);
            cmd.Parameters.AddWithValue("@notas", string.IsNullOrEmpty(request.Notas) ? DBNull.Value : request.Notas);
            cmd.ExecuteNonQuery();
        }
        return Ok(new { mensaje = "Equipo registrado exitosamente" });
    }

    [HttpPut("{id}")]
    public IActionResult ActualizarEquipo(int id, [FromBody] EquipoRequest request)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = @"UPDATE Equipos SET Tipo=@tipo, Marca=@marca, Modelo=@modelo, 
                            NumeroSerie=@serie, Estado=@estado, FechaCompra=@fechaCompra, 
                            Garantia=@garantia, Notas=@notas WHERE Id=@id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tipo", request.Tipo);
            cmd.Parameters.AddWithValue("@marca", request.Marca);
            cmd.Parameters.AddWithValue("@modelo", request.Modelo);
            cmd.Parameters.AddWithValue("@serie", request.Serie);
            cmd.Parameters.AddWithValue("@estado", request.Estado);
            cmd.Parameters.AddWithValue("@fechaCompra", request.FechaCompra);
            cmd.Parameters.AddWithValue("@garantia", string.IsNullOrEmpty(request.Garantia) ? DBNull.Value : request.Garantia);
            cmd.Parameters.AddWithValue("@notas", string.IsNullOrEmpty(request.Notas) ? DBNull.Value : request.Notas);
            cmd.ExecuteNonQuery();
        }
        return Ok(new { mensaje = "Equipo actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public IActionResult EliminarEquipo(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = "DELETE FROM Equipos WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        return Ok(new { mensaje = "Equipo eliminado exitosamente" });
    }
}

public class EquipoRequest
{
    public string Tipo { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Modelo { get; set; } = "";
    public string Serie { get; set; } = "";
    public string Estado { get; set; } = "";
    public string FechaCompra { get; set; } = "";
    public string? Garantia { get; set; }
    public string? Notas { get; set; }
}