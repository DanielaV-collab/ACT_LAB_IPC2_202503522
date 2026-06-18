# Parte 1: Investigación Teórica y Análisis de Casos

**Estudiante:** Daniela Odeth Velásquez Solís  
**Carnet:** 202503522
**Repositorio**:[ACT_LAB_IPC2_202503522](https://github.com/DanielaV-collab/ACT_LAB_IPC2_202503522.git)

Con base en los conceptos de rotaciones dobles en árboles AVL y la teoría de servicios web, se presenta el siguiente análisis detallado:

## 1. El Límite de las Rotaciones Simples y Desbalanceo en "Zig-Zag"

### El Problema Cruzado
Las rotaciones simples (como RLL o RRD) están diseñadas para corregir desbalances puramente lineales ("en línea recta"). Cuando se insertan secuencias cruzadas en forma de "Zig-Zag" (como la secuencia 30, 10, 20), las rotaciones simples fallan debido a que solo cambian la inclinación de la estructura de un lado hacia el otro, sin reducir la altura máxima del subárbol ni resolver el problema de fondo.

### Condición Matemática de Activación para una Rotación Doble Izquierda-Derecha (RID)
El Factor de Equilibrio ($FE$) de un nodo en un árbol AVL determina el estado de balance de la estructura. Una Rotación Doble Izquierda-Derecha (RID) se gatilla específicamente cuando se cumplen simultáneamente las siguientes condiciones matemáticas en los factores de equilibrio del nodo padre (abuelo) y del nodo hijo izquierdo:

1. **Nodo Padre:** $$FE = -2$$ 
2. **Nodo Hijo Izquierdo:** $$FE = +1$$ 

Esta combinación de signos opuestos (un desbalance hacia la izquierda en el padre con una inclinación hacia la derecha en su hijo) define analíticamente el caso cruzado.

### Principio DRY (Don't Repeat Yourself)
El principio DRY (No te repitas) es una regla fundamental en la ingeniería de software orientada a la eficiencia y modularidad. 

La ventaja clave de implementar las operaciones compuestas (como RID y RDI) reutilizando las primitivas de rotación simple existentes, en lugar de reasignar punteros manualmente desde cero, radica en:
* **Mitigación de Errores:** Evita tener que manipular manualmente múltiples enlaces de memoria de forma directa, disminuyendo drásticamente el riesgo de perder referencias de nodos o generar ciclos infinitos.
* **Mantenibilidad y Limpieza:** Si las funciones de rotación simple ya han sido probadas y están libres de errores, construir las rotaciones dobles como una secuencia lógica de operaciones simples garantiza un código altamente confiable, legible y fácil de mantener.

---

## 2. Fundamentos de Arquitectura Web y Protocolo HTTP

### El Modelo Cliente-Servidor
En la transición hacia arquitecturas de servicios web, la comunicación se rige mediante la interacción de dos componentes esenciales:

* **El Cliente Web:** Componente (como un navegador o aplicación) encargado de iniciar la comunicación mediante la creación de una solicitud o petición (Request) orientada a un recurso específico en el servidor.
* **El Servidor Web:** Componente que se encuentra a la escucha de peticiones, las procesa y devuelve una respuesta estructurada (Response) al cliente.

**Flujo de los Datos:**
* En una **Petición (Request)**, los datos viajan desde el cliente organizados a través de la línea de solicitud (Método HTTP, Ruta), encabezados (Headers) y opcionalmente un cuerpo (Body) en formatos estándar como JSON.
* En una **Respuesta (Response)**, la información viaja de regreso desde el servidor al cliente incorporando códigos de estado HTTP (que reflejan el resultado de la operación), encabezados de control y el cuerpo con el estado físico actual del recurso solicitado.

### Semántica de Operaciones: GET vs. POST

La distinción técnica entre los métodos HTTP GET y POST se define bajo los siguientes criterios semánticos:

* **HTTP GET:** Es un método diseñado exclusivamente para la **recuperación de la estructura de datos**. Es una operación de lectura que no debe alterar ni mutar el estado de la información almacenada en el servidor. En esta simulación, se utiliza para consultar la composición actual del árbol en memoria.
* **HTTP POST:** Es un método diseñado para la **mutación o inserción de nuevos elementos** en el servidor. Es una operación de escritura que envía información estructurada en el cuerpo de la petición para crear un recurso nuevo o desencadenar una lógica de negocio (como el balanceo tras añadir un nodo).