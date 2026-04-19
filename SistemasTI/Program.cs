var builder = WebApplication.CreateBuilder(args);

// 1. Agregar Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Esto es OpenAPI

// 2. IMPORTANTE: Habilitar CORS para que tu Dashboard (puerto 5500) pueda entrar
builder.Services.AddCors(options => {
    options.AddPolicy("PermitirFrontend", policy => {
        policy.AllowAnyOrigin() // O puedes poner "http://127.0.0.1:5500"
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 3. Configurar Swagger (OpenAPI)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Esto genera la página de pruebas
}

app.UseCors("PermitirFrontend"); // Aplicar los permisos
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
