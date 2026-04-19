using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly string? _connectionString;

    public MantenimientoController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    [HttpGet]
    public IActionResult GetMantenimientos()
    {
        List<object> mantenimientos = new List<object>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = @"SELECT m.Id, e.Marca, e.Modelo, e.NumeroSerie,
                            m.Tipo, m.Descripcion, m.Tecnico, m.Fecha, 
                            m.Costo, m.ProximoMantenimiento
                            FROM Mantenimientos m
                            INNER JOIN Equipos e ON m.EquipoId = e.Id
                            ORDER BY m.Fecha DESC";
            SqlCommand cmd = new SqlCommand(query, conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    mantenimientos.Add(new
                    {
                        id = reader["Id"],
                        marca = reader["Marca"]?.ToString(),
                        modelo = reader["Modelo"]?.ToString(),
                        serieEquipo = reader["NumeroSerie"]?.ToString(),
                        tipo = reader["Tipo"]?.ToString(),
                        descripcion = reader["Descripcion"]?.ToString(),
                        tecnico = reader["Tecnico"]?.ToString(),
                        fecha = reader["Fecha"] != DBNull.Value ?
    Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd") : "",
                        costo = reader["Costo"]?.ToString(),
                        proximoMantenimiento = reader["ProximoMantenimiento"]?.ToString()
                    });
                }
            }
        }
        return Ok(mantenimientos);
    }

    [HttpPost]
    public IActionResult CrearMantenimiento([FromBody] MantenimientoRequest request)
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

            // Insertar mantenimiento
            string query = @"INSERT INTO Mantenimientos (EquipoId, Tipo, Descripcion, Tecnico, Fecha, Costo, ProximoMantenimiento)
                            VALUES (@equipoId, @tipo, @descripcion, @tecnico, @fecha, @costo, @proximoMantenimiento)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@equipoId", equipoId);
            cmd.Parameters.AddWithValue("@tipo", request.Tipo);
            cmd.Parameters.AddWithValue("@descripcion", request.Descripcion);
            cmd.Parameters.AddWithValue("@tecnico", request.Tecnico);
            cmd.Parameters.AddWithValue("@fecha", request.Fecha);
            cmd.Parameters.AddWithValue("@costo", string.IsNullOrEmpty(request.Costo) ? DBNull.Value : request.Costo);
            cmd.Parameters.AddWithValue("@proximoMantenimiento", string.IsNullOrEmpty(request.ProximoMantenimiento) ? DBNull.Value : request.ProximoMantenimiento);
            cmd.ExecuteNonQuery();

            // Si es correctivo actualizar estado del equipo
            if (request.Tipo == "Correctivo")
            {
                string updateEquipo = "UPDATE Equipos SET Estado = 'Mantenimiento' WHERE NumeroSerie = @serie";
                SqlCommand cmdUpdate = new SqlCommand(updateEquipo, conn);
                cmdUpdate.Parameters.AddWithValue("@serie", request.SerieEquipo);
                cmdUpdate.ExecuteNonQuery();
            }
        }
        return Ok(new { mensaje = "Mantenimiento registrado exitosamente" });
    }
}

public class MantenimientoRequest
{
    public string SerieEquipo { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Tecnico { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string? Costo { get; set; }
    public string? ProximoMantenimiento { get; set; }
}