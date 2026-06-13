using ApiAvlSimulacion;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Simulación del estado del árbol en memoria RAM utilizando una lista secuencial
var estadoArbol = new List<NodoAVL>
{
    // Estado inicial desbalanceado en forma de Zig-Zag (Escenario de la Slide 5)
    new NodoAVL { Id = 30, Etiqueta = "Nodo Raiz (Abuelo) - FE: -2", Altura = 3 },
    new NodoAVL { Id = 10, Etiqueta = "Hijo Izquierdo - FE: +1", Altura = 2 }
};

// 1. ENDPOINT GET: Recupera la estructura física del árbol actual en memoria
app.MapGet("/api/arbol", () => {
    return Results.Ok(estadoArbol);
});

// 2. ENDPOINT POST: Simula la inserción de un elemento y el disparo del balanceo compuesto
app.MapPost("/api/arbol/insertar", (NodoAVL nuevoNodo) =>
{
    // Validación de seguridad básica para la llave
    if (nuevoNodo.Id <= 0)
    {
        return Results.BadRequest("ID de nodo inválido.");
    }

    // Simulación del motor de inserción: Al insertar el número 20, se detecta el caso cruzado
    if (nuevoNodo.Id == 20)
    {
        estadoArbol.Clear(); // Vaciamos el estado viejo desbalanceado
        
        // El resultado de aplicar la rotación doble izquierda-derecha (RID) equilibra perfectamente el subárbol
        estadoArbol.Add(new NodoAVL { Id = 20, Etiqueta = "Nueva Raiz Balanceada (RID) - FE: 0", Altura = 2 });
        estadoArbol.Add(new NodoAVL { Id = 10, Etiqueta = "Hijo Izquierdo - FE: 0", Altura = 1 });
        estadoArbol.Add(new NodoAVL { Id = 30, Etiqueta = "Hijo Derecho - FE: 0", Altura = 1 });

        return Results.Created("/api/arbol", new 
        { 
            Mensaje = "Rotación RID ejecutada con éxito. Estabilidad total lograda.", 
            Estructura = estadoArbol 
        });
    }

    // Inserción tradicional lineal en caso de que no sea la llave del escenario crítico
    estadoArbol.Add(nuevoNodo);
    return Results.Created($"/api/arbol/{nuevoNodo.Id}", nuevoNodo);
});

app.Run();

