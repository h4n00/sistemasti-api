using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly string? _connectionString;

    public UsuarioController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string contrasenaHash = HashPassword(request.Contrasena);
            string query = "SELECT Id, Nombre, Email, Rol FROM Usuarios WHERE Email = @email AND Contrasena = @contrasena";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", request.Email);
            cmd.Parameters.AddWithValue("@contrasena", contrasenaHash);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Ok(new
                    {
                        id = reader["Id"],
                        nombre = reader["Nombre"]?.ToString(),
                        email = reader["Email"]?.ToString(),
                        rol = reader["Rol"]?.ToString()
                    });
                }
            }
        }
        return Unauthorized("Credenciales incorrectas");
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Contrasena { get; set; } = "";
}