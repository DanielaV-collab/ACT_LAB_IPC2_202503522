using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 3. Configuración del almacenamiento en memoria (Lista estática)
// Inicializada con los dos nodos de ejemplo requeridos
var coleccionNodos = new List<NodoElemento>
{
    new NodoElemento { Id = 10, Valor = "Raíz Inicial (ABB)" },
    new NodoElemento { Id = 5, Valor = "Hijo Izquierdo" }
};

// 4. Implementación de Endpoints

// Endpoint 1: GET - Retorna todos los nodos actuales
app.MapGet("/api/nodos", () => Results.Ok(coleccionNodos));

// Endpoint 2: POST - Recibe un nuevo nodo y lo "inserta" en la colección
app.MapPost("/api/nodos", ([FromBody] NodoElemento nuevoNodo) =>
{
    // Validación simple de los datos recibidos
    if (nuevoNodo.Id <= 0 || string.IsNullOrEmpty(nuevoNodo.Valor))
    {
        return Results.BadRequest("Datos del nodo inválidos.");
    }

    // Agregar el nodo a nuestra lista estática en memoria
    coleccionNodos.Add(nuevoNodo);

    // Retorna un código 201 Created junto con la ubicación y el objeto creado
    return Results.Created($"/api/nodos/{nuevoNodo.Id}", nuevoNodo);
});

// 5. Ejecución de la API
app.Run();

// 2. Modelado del Recurso (La clase "Nodo")
public class NodoElemento
{
    public int Id { get; set; }
    public string Valor { get; set; } = string.Empty;
}
