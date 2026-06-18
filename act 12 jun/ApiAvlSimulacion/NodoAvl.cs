namespace ApiAvlSimulacion;

public class NodoAVL
{
    public int Id { get; set; } // Actúa como el Dato/Llave de ordenamiento [cite: 28]
    public string Etiqueta { get; set; } = string.Empty; // Descripción informativa del estado [cite: 29]
    public int Altura { get; set; } = 1; // Control interno de la altura del nodo [cite: 30]
}

