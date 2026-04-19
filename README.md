# SistemasTI - API REST

API REST desarrollada en .NET 8 para el Sistema de Inventario de Equipos de Cómputo. Gestiona la autenticacion de usuarios, equipos, asignaciones y mantenimientos conectandose a una base de datos SQL Server.

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Microsoft.Data.SqlClient
- Swagger / OpenAPI
- SQL Server

## Requisitos previos

- .NET SDK 8 o superior
- SQL Server 2019 o superior
- Visual Studio 2022 

## Instalacion

Clona el repositorio:

bash
git clone https://github.com/h4n00/sistemasti-api.git
cd sistemasti-api


## Configuracion de base de datos

Crea la base de datos en SQL Server:

sql
CREATE DATABASE SistemaInventario;

Crea las tablas ejecutando los siguientes scripts:

sql
USE SistemaInventario;

CREATE TABLE Usuarios (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Nombre NVARCHAR(100) NOT NULL,
  Email NVARCHAR(100) NOT NULL,
  Rol NVARCHAR(20) NOT NULL,
  Contrasena NVARCHAR(256) NOT NULL
);

CREATE TABLE Equipos (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Tipo NVARCHAR(50) NOT NULL,
  Marca NVARCHAR(50) NOT NULL,
  Modelo NVARCHAR(100) NOT NULL,
  NumeroSerie NVARCHAR(100) NOT NULL UNIQUE,
  Estado NVARCHAR(20) NOT NULL,
  FechaCompra DATE NOT NULL,
  Garantia DATE NULL,
  Notas NVARCHAR(500) NULL
);

CREATE TABLE Asignaciones (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  EquipoId INT NOT NULL,
  Usuario NVARCHAR(100) NOT NULL,
  Area NVARCHAR(100) NOT NULL,
  FechaAsignacion DATE NOT NULL,
  Notas NVARCHAR(500) NULL,
  CONSTRAINT FK_Asignaciones_Equipos FOREIGN KEY (EquipoId) REFERENCES Equipos(Id) ON DELETE CASCADE
);

CREATE TABLE Mantenimientos (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  EquipoId INT NOT NULL,
  Tipo NVARCHAR(20) NOT NULL,
  Descripcion NVARCHAR(500) NOT NULL,
  Tecnico NVARCHAR(100) NOT NULL,
  Fecha DATE NOT NULL,
  Costo DECIMAL(10,2) NULL,
  ProximoMantenimiento DATE NULL,
  CONSTRAINT FK_Mantenimientos_Equipos FOREIGN KEY (EquipoId) REFERENCES Equipos(Id) ON DELETE CASCADE
);

Inserta los usuarios de prueba:

sql
INSERT INTO Usuarios (Nombre, Email, Rol, Contrasena) VALUES
('Admin Principal', 'admin@inventario.com', 'administrador', CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', 'password123'), 2)),
('Tecnico Principal', 'tecnico@inventario.com', 'tecnico', CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', 'password123'), 2)),
('Usuario Principal', 'usuario@inventario.com', 'usuario', CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', 'password123'), 2));

## Configuracion de conexion

Copia el archivo de ejemplo y configura tu servidor:

bash
cp appsettings.example.json appsettings.json

Edita `appsettings.json` y cambia `TU_SERVIDOR` por el nombre de tu servidor SQL Server.
## Paquetes NuGet

| Paquete | Descripcion |
|---------|-------------|
| Microsoft.Data.SqlClient | Conexion y consultas a SQL Server |
| Swashbuckle.AspNetCore | Generacion de documentacion Swagger |

## Ejecucion

Desde Visual Studio 2022 presiona F5 o ejecuta:

bash
dotnet run

La API estara disponible en o dependiendo de tu computadora`http://localhost:5228`

La documentacion Swagger estara en `http://localhost:5228/swagger`

## Endpoints disponibles

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/usuario/login | Autenticacion de usuario |
| GET | /api/equipo | Obtener todos los equipos |
| POST | /api/equipo | Registrar nuevo equipo |
| PUT | /api/equipo/{id} | Actualizar equipo |
| DELETE | /api/equipo/{id} | Eliminar equipo |
| GET | /api/asignacion | Obtener todas las asignaciones |
| POST | /api/asignacion | Crear nueva asignacion |
| GET | /api/mantenimiento | Obtener todos los mantenimientos |
| POST | /api/mantenimiento | Registrar mantenimiento |

## Repositorio del frontend

https://github.com/h4n00/sistemasti
