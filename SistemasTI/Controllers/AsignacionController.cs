using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class AsignacionController : ControllerBase
{
    private readonly string? _connectionString;

    public AsignacionController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    [HttpGet]
    public IActionResult GetAsignaciones()
    {
        List<object> asignaciones = new List<object>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = @"SELECT a.Id, e.Marca, e.Modelo, e.NumeroSerie, 
                            a.Usuario, a.Area, a.FechaAsignacion, a.Notas
                            FROM Asignaciones a
                            INNER JOIN Equipos e ON a.EquipoId = e.Id";
            SqlCommand cmd = new SqlCommand(query, conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    asignaciones.Add(new
                    {
                        id = reader["Id"],
                        marca = reader["Marca"]?.ToString(),
                        modelo = reader["Modelo"]?.ToString(),
                        serieEquipo = reader["NumeroSerie"]?.ToString(),
                        usuario = reader["Usuario"]?.ToString(),
                        area = reader["Area"]?.ToString(),
                        fecha = reader["FechaAsignacion"] != DBNull.Value ?
    Convert.ToDateTime(reader["FechaAsignacion"]).ToString("yyyy-MM-dd") : "",
                        notas = reader["Notas"]?.ToString()
                    });
                }
            }
        }
        return Ok(asignaciones);
    }

    [HttpPost]
    public IActionResult CrearAsignacion([FromBody] AsignacionRequest request)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            // Obtener el Id del equipo
            string queryEquipo = "SELECT Id FROM Equipos WHERE NumeroSerie = @serie";
            SqlCommand cmdEquipo = new SqlCommand(queryEquipo, conn);
            cmdEquipo.Parameters.AddWithValue("@serie", request.SerieEquipo);
            var equipoId = cmdEquipo.ExecuteScalar();

            if (equipoId == null)
                return NotFound("Equipo no encontrado");

            // Insertar asignacion
            string query = @"INSERT INTO Asignaciones (EquipoId, Usuario, Area, FechaAsignacion, Notas)
                            VALUES (@equipoId, @usuario, @area, @fecha, @notas)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@equipoId", equipoId);
            cmd.Parameters.AddWithValue("@usuario", request.Usuario);
            cmd.Parameters.AddWithValue("@area", request.Area);
            cmd.Parameters.AddWithValue("@fecha", request.Fecha);
            cmd.Parameters.AddWithValue("@notas", string.IsNullOrEmpty(request.Notas) ? DBNull.Value : request.Notas);
            cmd.ExecuteNonQuery();

            // Actualizar estado del equipo
            string updateEquipo = "UPDATE Equipos SET Estado = 'Asignado' WHERE NumeroSerie = @serie";
            SqlCommand cmdUpdate = new SqlCommand(updateEquipo, conn);
            cmdUpdate.Parameters.AddWithValue("@serie", request.SerieEquipo);
            cmdUpdate.ExecuteNonQuery();
        }
        return Ok(new { mensaje = "Equipo asignado exitosamente" });
    }
}

public class AsignacionRequest
{
    public string SerieEquipo { get; set; } = "";
    public string Usuario { get; set; } = "";
    public string Area { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string? Notas { get; set; }
}