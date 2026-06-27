# Actividad de Laboratorio: Interoperabilidad y Carga Masiva de Datos

**Estudiante:** Daniela Velásquez  
**carnet:** 202503522 

**Curso:** Lab Introducción a la Programación y Computación 2  

---

## Parte 1: Evaluación Conceptual y Buenas Prácticas

### Formatos de Intercambio: Tabla Comparativa

| Formato | Ventajas | Desventajas |
| :--- | :--- | :--- |
| **CSV** | • Formato extremadamente ligero y simple.<br>• Mínimo consumo de ancho de banda y almacenamiento.<br>• Fácil de leer y generar desde cualquier lenguaje o herramienta (como Excel). | • No soporta estructuras de datos complejas o anidadas (es estrictamente plano).<br>• Carece de un estándar estricto de tipado de datos o codificación nativa.<br>• No incluye metadatos para validar la estructura automáticamente. |
| **XML** | • Permite representar estructuras de datos complejas, jerárquicas y anidadas.<br>• Altamente estandarizado y compatible con validaciones estrictas de esquema (XSD).<br>• Es autodocumentado gracias al uso de etiquetas personalizadas. | • Produce archivos muy pesados debido a la redundancia de las etiquetas de apertura y cierre.<br>• Mayor consumo de memoria RAM y CPU para su procesamiento (parsing).<br>• Menos eficiente para la transmisión masiva de datos en comparación con JSON o CSV. |

### 1. Diferenciación de Procesos: Serialización y Deserialización

Utilizando la librería nativa `System.Text.Json`, la diferencia técnica radica en la dirección del flujo de los datos y su transformación:

* **Serialización:** Es el proceso de transformar un objeto en memoria de C# (una instancia de una clase) en una secuencia de bytes o una cadena de texto en formato JSON. Se utiliza principalmente para enviar datos a través de una red o almacenarlos en un archivo. En `System.Text.Json`, esto se logra mediante el método `JsonSerializer.Serialize()`.
* **Deserialización:** Es el proceso inverso. Consiste en tomar una cadena de texto o un flujo de bytes en formato JSON y reconstruir a partir de él un objeto fuertemente tipado en la memoria de C#. Se emplea al recibir la respuesta de una API o leer un archivo de configuración. En la librería, se ejecuta utilizando el método `JsonSerializer.Deserialize<T>()`.

### 2. El Antipatrón del Rendimiento: Problema "N+1" y Estrategia de Batching

* **El Problema "N+1":** Ocurre durante la lectura de un archivo masivo cuando el sistema realiza una consulta o inserción individual en la base de datos por cada registro leído. Si el archivo tiene $N$ filas, se ejecutarán $N$ transacciones o consultas individuales a la base de datos (más la consulta inicial, sumando $N+1$). Esto genera una sobrecarga crítica en la red y degrada drásticamente el rendimiento del servidor.
* **Estrategia de Optimización (Batching):** Para solucionarlo, se aplica el procesamiento por lotes. En lugar de comunicarse con la base de datos fila por fila, los registros mapeados se acumulan temporalmente en una lista en memoria. Al finalizar el ciclo de lectura (o al alcanzar un tamaño de lote óptimo), se realiza una **única operación masiva** utilizando `AddRange()` y se confirma con una sola llamada asíncrona a `SaveChangesAsync()`, reduciendo las conexiones a la base de datos a un mínimo eficiente.

---

## Parte 2: Implementación Práctica en C#

### Desafío 1: Consumo de Endpoints y Deserialización

A continuación, se presenta la implementación del método asíncrono utilizando `HttpClient` y las directrices de configuración solicitadas:

```csharp
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Alumno
{
    // Define aquí las propiedades correspondientes a la entidad Alumno de la USAC
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Carne { get; set; }
}

public class ConectividadAcademica
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<Alumno[]> ObtenerAlumnosAsync()
    {
        string url = "[https://api.usac.edu/v1/alumnos](https://api.usac.edu/v1/alumnos)";

        try
        {
            // Realizar la petición segura de tipo GET
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            
            // Validar el código de estado HTTP (lanza excepción si no es 2xx)
            response.EnsureSuccessStatusCode();

            // Leer el contenido de la respuesta como una cadena de texto
            string jsonPayload = await response.Content.ReadAsStringAsync();

            // Configurar las opciones para habilitar la insensibilidad a mayúsculas y minúsculas
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserializar el payload JSON al objeto/arreglo correspondiente
            Alumno[] alumnos = JsonSerializer.Deserialize<Alumno[]>(jsonPayload, options);

            return alumnos;
        }
        catch (HttpRequestException ex)
        {
            // Manejo de errores de comunicación HTTP o códigos de estado no exitosos
            Console.WriteLine($"Error de comunicación con la API de la USAC: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            // Manejo de errores estrictamente vinculados a la deserialización del formato JSON
            Console.WriteLine($"Error al deserializar el payload JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Manejo de cualquier otra excepción imprevista
            Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
            throw;
        }
    }
}

```


### Desafío 2: Endpoint para Carga Masiva CSV

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CargaMasivaController : ControllerBase
{
    // Reemplaza 'DbContext' por el nombre real del contexto de datos de tu proyecto
    private readonly DbContext _context; 

    public CargaMasivaController(DbContext context)
    {
        _context = context;
    }

    [HttpPost("cargar-alumnos")]
    public async Task<IActionResult> CargarAlumnosCsv(IFormFile archivoCsv)
    {
        // Validar que el archivo no sea nulo ni esté vacío
        if (archivoCsv == null || archivoCsv.Length == 0)
        {
            return BadRequest("El archivo proporcionado no es válido o está vacío.");
        }

        // Lista intermedia para almacenar los registros procesados en memoria
        var listaAlumnos = new List<Alumno>();

        try
        {
            // Utilizar StreamReader para procesar el archivo de forma asíncrona y eficiente
            using (var stream = archivoCsv.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                string linea;
                // Leer línea por línea evitando la saturación de la memoria RAM
                while ((linea = await reader.ReadLineAsync()) != null)
                {
                    // Saltar líneas vacías
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    // Parsear o mapear la línea CSV (Suponiendo formato: Nombre,Carne)
                    var columnas = linea.Split(',');
                    
                    if (columnas.Length >= 2)
                    {
                        var nuevoAlumno = new Alumno
                        {
                            Nombre = columnas[0].Trim(),
                            Carne = columnas[1].Trim()
                        };

                        listaAlumnos.Add(nuevoAlumno);
                    }
                }
            }

            // Aplicar estrategia de optimización por lotes (Batching)
            if (listaAlumnos.Count > 0)
            {
                // Inserción completa en la base de datos en un solo bloque
                await _context.AddRangeAsync(listaAlumnos);
                
                // Una única llamada asíncrona para confirmar la transacción
                await _context.SaveChangesAsync();
            }

            return Ok(new { Mensaje = $"Carga masiva finalizada con éxito. Se registraron {listaAlumnos.Count} alumnos." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno durante el procesamiento de la carga masiva: {ex.Message}");
        }
    }
}

```
### Referencias Bibliograficas

Facultad de Ingeniería, USAC. (2026). Sesión 20: Integración de Datos. Consumo de APIs Externas y Carga Masiva (CSV/XML). Laboratorio del curso Introducción a la Programación y Computación 2. Guatemala.

