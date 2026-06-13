# Actividad de Investigación y Práctica: Estructuras de Datos Avanzadas y APIs con ASP.NET Core

**Nombre:** Daniela Odeth Velásquez Solís  
**Carné:** 202503522  
**Repositorio de Código:** [https://github.com/DanielaV-collab/ACT_LAB_IPC2.git](https://github.com/DanielaV-collab/ACT_LAB_IPC2.git)  

---

## Parte 1: Investigación Teórica

### 1. Estructuras de Datos Eficientes

#### **Árboles Binarios de Búsqueda (ABB)**
* **Regla de ordenamiento:** Es una estructura jerárquica donde cada nodo puede tener como máximo dos hijos. Su regla principal dicta que para cualquier nodo dado, todos los elementos en su **hijo izquierdo** deben tener un valor **menor**, y todos los elementos en su **hijo derecho** deben tener un valor **mayor**.
* **Principal desventaja (Degeneración en lista vinculada):** Cuando los datos se insertan en un orden estrictamente secuencial o ya ordenado (por ejemplo: 1, 2, 3, 4, 5), el ABB pierde su forma de árbol y se **degenera en una lista vinculada** (adquiere una forma lineal hacia un solo lado). Al ocurrir esto, la ventaja de la búsqueda rápida desaparece, y la complejidad del tiempo de operación pasa de ser eficiente a volverse lineal, es decir, $O(n)$.

#### **Árboles AVL**
* **Definición de árbol auto-balanceado:** Es un Árbol Binario de Búsqueda especial que se reorganiza de forma automática mediante operaciones llamadas "rotaciones" cada vez que se inserta o elimina un elemento, asegurando que la estructura se mantenga lo más simétrica y compacta posible.
* **Factor de balanceo:** Se calcula para cada nodo restando la altura de sus subárboles: 
  $$Factor = Altura_{Izquierda} - Altura_{Derecha}$$
  Para que el árbol se considere balanceado, este factor en cada nodo debe ser estrictamente **-1, 0 o 1**. Si llega a ser $\ge 2$ o $\le -2$, el árbol ejecuta una rotación para equilibrarse.
* **Complejidad $O(\log n)$:** Debido a que el árbol se mantiene estrictamente balanceado de forma constante, la altura máxima del árbol siempre está acotada de forma logarítmica respecto al número de nodos ($n$). Como el peor de los caminos posibles para buscar, insertar o eliminar un dato es igual a la altura del árbol, estas operaciones se garantizan siempre en una complejidad de tiempo de **$O(\log n)$**.

---

### 2. Fundamentos de Web APIs

#### **¿Qué es una API y cómo funciona el modelo Cliente-Servidor?**
* **API (Application Programming Interface):** Es un conjunto de reglas y definiciones que permite que dos aplicaciones o sistemas de software diferentes se comuniquen e intercambien datos entre sí de forma estructurada.
* **Modelo Cliente-Servidor:** Es un modelo de diseño de software donde las tareas se reparten entre los proveedores de recursos o servicios (**Servidores**) y los demandantes de dichos servicios (**Clientes**). El cliente inicia la comunicación enviando una solicitud a través de la red, y el servidor la procesa para devolver un resultado.

#### **Viaje de una petición (Request) y respuesta (Response) en HTTP**
1. **Petición (Request):** El cliente (por ejemplo, Postman o un navegador web) empaqueta una solicitud HTTP que incluye una dirección URL, un **Verbo/Método HTTP** (como GET o POST), **Cabeceras** (Headers) con metadatos (como el tipo de contenido) y opcionalmente un **Cuerpo** (Body) con datos en formato JSON. Esta petición viaja por la red mediante el protocolo TCP/IP hasta el servidor.
2. **Procesamiento:** El servidor web (en este caso, nuestra API en ASP.NET Core) recibe la petición, interpreta la ruta, ejecuta la lógica de negocio correspondiente (como leer o añadir datos a una lista) y prepara un resultado.
3. **Respuesta (Response):** El servidor empaqueta una respuesta HTTP que contiene un **Código de Estado** (Status Code, como `200 OK` o `201 Created`), cabeceras informativas y, por lo general, un cuerpo de respuesta (Body) con el JSON solicitado. Este paquete vuelve a viajar por la red hacia el cliente que lo solicitó.

#### **Diferencia conceptual, uso e idempotencia de Verbos HTTP**

| Verbo HTTP | Uso Correcto | ¿Es Idempotente? | Explicación |
| :--- | :--- | :--- | :--- |
| **GET** | Recuperación y consulta de recursos existentes sin modificar el servidor. | **SÍ** | Es idempotente porque realizar la misma petición GET 1 o 100 veces seguidas devolverá el mismo resultado y no cambiará el estado del sistema ni creará efectos secundarios en el almacenamiento. |
| **POST** | Creación de un nuevo recurso en el servidor. | **NO** | No es idempotente porque si envías la misma petición POST 5 veces consecutivas con los mismos datos, el servidor creará 5 registros o recursos diferentes de manera repetida. |

---
