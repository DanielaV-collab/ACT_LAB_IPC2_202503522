# Reporte de Laboratorio: Arquitectura Multi-Nivel (N-Tier) y Patrón MVC

**Estudiante:** Daniela Velásquez  
**Carné:** 202503522  
**Facultad:** Ingeniería, Universidad de San Carlos de Guatemala (USAC)  
**Curso:** Lab Introducción a la Programación y Computación 2  

---

## Parte 1: Fundamentación Teórica y Análisis Crítico

### 1. El Tránsito hacia los Sistemas Distribuidos y Multi-Capa

#### La Limitación del Monolito Local
Cuando la interfaz de usuario, la lógica de negocio y el almacenamiento de datos residen de forma exclusiva dentro de una única máquina física aislada, ocurren los siguientes problemas críticos en la ingeniería de software:

* **Sincronización de Datos:** Se vuelve imposible centralizar o compartir la información en tiempo real. Si múltiples usuarios necesitasen interactuar con el sistema, cada máquina física aislada mantendría un estado de datos propio y fragmentado, rompiendo la consistencia de la información y haciendo inviable la consolidación de un único registro de la verdad de los datos.
* **Escalabilidad:** El sistema queda completamente encasillado en el **escalamiento vertical** (añadir componentes de hardware más costosos como RAM o CPU a esa única máquina). Se pierde la capacidad de realizar un **escalamiento horizontal** (distribuir las solicitudes de los usuarios entre múltiples servidores intermedios). Ante un incremento masivo en la demanda o un fallo de hardware en el equipo físico, el servicio sufrirá una caída total del sistema sin opciones de redundancia.

#### Distinción Crítica (Layers vs. Tiers)
Aunque coloquialmente se les llame "capas", existe una diferencia conceptual y de despliegue muy estricta entre ambas:

* **Capas Lógicas (Layers):** Hacen referencia a la separación de responsabilidades y organización **dentro del código fuente** del software. Ayudan al desarrollador a mantener el orden estructural (por ejemplo: separar las clases de datos de las de negocio), pero se ejecutan juntas dentro del mismo proceso y espacio de memoria en una única máquina.
* **Niveles Físicos (Tiers):** Hacen referencia a la distribución física y real de los componentes del software en **plataformas informáticas y servidores de hardware independientes**, los cuales se comunican obligatoriamente a través de una red.

#### Responsabilidades en la Arquitectura de 3 Niveles
A continuación se describe la misión exclusiva y la tecnología común empleada en cada uno de los niveles físicos:

* **Nivel 1: Capa de Presentación (Presentation Tier):**
  * **Misión Exclusiva:** Proveer la interfaz gráfica con la que interactúa el usuario final, capturar sus entradas o peticiones y renderizar los resultados de regreso de forma visual.
  * **Tecnología Común:** Navegadores web modernos ejecutando estructuras en HTML5, CSS3 y JavaScript (o frameworks SPA como Angular, React, Vue), así como aplicaciones móviles nativas.
* **Nivel 2: Capa de Aplicación o Negocio (Application Tier):**
  * **Misión Exclusiva:** Albergar el núcleo lógico y operativo del sistema. Se encarga de ejecutar algoritmos, procesar reglas de negocio, validar la seguridad y coordinar el intercambio de datos entre la presentación y la base de datos.
  * **Tecnología Común:** Servidores de aplicaciones y APIs construidas sobre ASP.NET Core (.NET 8) en C#, Spring Boot (Java), Node.js o Django (Python).
* **Nivel 3: Capa de Datos (Data Tier):**
  * **Misión Exclusiva:** Almacenar, persistir, indexar y asegurar la integridad de la información del dominio del software.
  * **Tecnología Común:** Motores de Sistemas de Gestión de Bases de Datos Relacionales (RDBMS) como SQL Server, PostgreSQL o MySQL, además de alternativas NoSQL como MongoDB.

#### Seguridad Perimetral
* **Justificación del Error Crítico:** Exponer el puerto de una base de datos directamente a internet (como el puerto `1433` de SQL Server o el `5432` de PostgreSQL) es un fallo de seguridad perimetral grave. Permite que cualquier actor malicioso o bot automatizado en la red escanee el puerto, intente ataques de fuerza bruta, realice denegaciones de servicio (DoS) o explote vulnerabilidades de día cero del motor, poniendo en riesgo crítico la confidencialidad de la información.
* **Buena Práctica Recomendada:** Aislar por completo el servidor del nivel de datos dentro de una red privada o subred protegida por un Firewall. La regla perimetral debe configurarse para aceptar conexiones entrantes **única y exclusivamente** desde la dirección IP interna del servidor de la Capa de Aplicación (Tier 2), bloqueando cualquier petición externa proveniente de internet.

### 2. Desacoplamiento Lógico con el Patrón MVC

#### La Crisis del Código Espagueti
Mezclar sentencias SQL, lógica matemática y etiquetas visuales dentro de un mismo archivo físico genera un acoplamiento destructivo con severos impactos negativos:

* **Mantenimiento del Software:** Cualquier modificación menor en el diseño de la interfaz visual (como cambiar una etiqueta HTML o un estilo CSS) puede corromper colateralmente la sintaxis de las consultas SQL o desajustar los algoritmos lógicos. Esto incrementa exponencialmente el riesgo de introducir errores inesperados (*bugs*) y eleva los costos operativos de mantenimiento a largo plazo.
* **Diseño de Pruebas Unitarias:** Se vuelve prácticamente imposible aislar la lógica matemática para probarla de forma automatizada. Dado que el código está fusionado con consultas de base de datos y elementos gráficos, una prueba unitaria fallará si no cuenta con una conexión de red activa al motor de datos o con un entorno de renderizado visual, rompiendo el principio de independencia de las pruebas de software.

#### Separación de Preocupaciones (SoC)
A continuación se detallan las características esenciales y el nivel de aislamiento de los tres componentes esenciales definidos por Trygve Reenskaug:

* **Modelo:**
  * **Qué representa:** Representa el estado del dominio, los datos puros y las reglas de negocio intrínsecas del sistema.
  * **Por qué no debe conocer cómo se muestran los datos:** Debe estar completamente aislado del entorno visual para garantizar su reutilización. El modelo procesa información sin importar si el resultado final se va a desplegar en una página web, una aplicación móvil, una consola de comandos o un servicio en la nube.
* **Vista:**
  * **Por qué es una entidad pasiva e inteligente:** Se define como pasiva porque no toma decisiones operacionales ni altera el estado de los datos; simplemente espera a que se le inyecte un modelo para transformarlo en una interfaz gráfica. Es inteligente porque sabe interpretar con precisión la sintaxis visual y las estructuras de diseño necesarias para renderizar la información de manera clara al usuario.
  * **Código prohibido:** Tiene estrictamente prohibido contener sentencias de acceso a bases de datos (consultas SQL), lógica de control compleja o algoritmos de negocio.
* **Controlador:**
  * **Rol en el sistema:** Actúa como el intermediario táctico y el director de orquesta del flujo de datos. Intercepta las peticiones HTTP entrantes enviadas desde el navegador, extrae y valida los parámetros de la solicitud, delega el procesamiento pesado al modelo correspondiente y, finalmente, selecciona y despacha la vista adecuada con los datos limpios para construir la respuesta.

#### Métricas de Ingeniería de Software
El patrón arquitectónico MVC es una de las herramientas más efectivas en el entorno profesional para optimizar las siguientes métricas:

* **Alta Cohesión:** Se logra garantizando que cada componente de software tenga una única responsabilidad bien definida. El controlador se limita a coordinar el tráfico , la vista a estructurar la interfaz visual y el modelo a gestionar el estado del dominio.
* **Bajo Acoplamiento:** Se alcanza reduciendo al mínimo la dependencia directa entre los componentes. Al comunicarse mediante interfaces claras y el paso de datos puros, se puede sustituir o rediseñar por completo el motor de vistas sin necesidad de alterar una sola línea de código en las consultas del modelo o en la lógica de control.

___
## Parte 2: Modelado del Ciclo de Vida y Enrutamiento Semántico

### 1. Mapeo Analítico de URLs

Dada la plantilla jerárquica convencional de ASP.NET Core `{controller=Home}/{action=Index}/{id?}`, se presenta la resolución estructural y sintáctica de las peticiones entrantes:

| URL Entrante del Cliente | Clase Controladora Buscada por el Framework | Método (Acción) Ejecutado | Parámetro Inyectado `id` |
| :--- | :--- | :--- | :--- |
| `https://ingenieria.usac.edu.gt/ControlAcademico/Login` | `ControlAcademicoController` | `Login` | *(Ninguno / Opcional)* |
| `https://ingenieria.usac.edu.gt/Estudiante/Historial/20260123` | `EstudianteController` | `Historial` | `20260123` |
| `https://ingenieria.usac.edu.gt/Asignacion/Detalle/10` | `AsignacionController` | `Detalle` | `10` |
| `https://ingenieria.usac.edu.gt/Home` | `HomeController` | `Index` | *(Ninguno / Opcional)* |
| `https://ingenieria.usac.edu.gt/` | `HomeController` | `Index` | *(Ninguno / Opcional)* |

### 2. Diagramación del Flujo Interactivo (Ciclo de Vida de una Petición)

A continuación se describe de forma secuencial y cronológica el viaje completo de una solicitud web desde el cliente hasta la generación de la respuesta dinámica:

1. **Petición del Cliente (HTTP Request):** El ciclo de vida inicia en el navegador web del usuario cuando este realiza una acción interactiva (como hacer clic en un botón de envío o ingresar a un enlace). El navegador empaqueta la solicitud en una petición HTTP (usando verbos como `GET` o `POST`) y la transmite a través de la red hacia el servidor donde se hospeda la aplicación.
2. **Intercepción y Enrutamiento (Routing Middleware):** Al llegar al servidor web, el motor de enrutamiento de ASP.NET Core intercepta la URL entrante. El sistema analiza los segmentos de la ruta basándose en las convenciones predefinidas, identifica dinámicamente qué clase controladora debe responder a la solicitud (por ejemplo, `EstudianteController`) y delega el control invocando el método de acción correspondiente.
3. **Orquestación en el Controlador (Controller):** El **Controlador** toma el control operacional de la solicitud como el intermediario táctico. Extrae, limpia y valida los parámetros de entrada inyectados por la URL o el cuerpo del mensaje. Una vez validada la periferia, manda a llamar o interactúa con los componentes correspondientes de la capa de dominio.
4. **Mutación u Obtención del Estado (Model):** El **Modelo** recibe las directrices del controlador y procesa de forma aislada las reglas de negocio, los cálculos lógicos o las operaciones de persistencia en memoria/base de datos. Tras completar la operación, el Modelo le devuelve objetos de datos puros u colecciones tipadas de C# al Controlador, el cual los intercepta y los inyecta inmediatamente dentro del método de despacho `View(modelo)`.
5. **Renderizado Dinámico y Respuesta (View -> HTTP Response):** El motor de vistas del Framework toma la plantilla `.cshtml` asociada (**Vista**) y, actuando de forma inteligente pero pasiva, combina las etiquetas de marcado estructural HTML con las propiedades dinámicas de los datos inyectados del Modelo. [cite_start]El resultado final se traduce en un flujo puro de código HTML dinámico, el cual es enviado de regreso a través de la red como una respuesta HTTP, permitiendo al navegador del usuario renderizar la interfaz visual final.


