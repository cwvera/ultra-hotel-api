# UltraHotel API

Backend de reservas hoteleras desarrollado en **.NET 10**. Gestiona hoteles, habitaciones y reservas con búsqueda en Elasticsearch, notificaciones asíncronas vía RabbitMQ y autenticación JWT por roles.

## TECNOLOGÍAS UTILIZADAS

.NET 10 - ASP.NET Core Web API - SQL Server 2022 - Elasticsearch 9 - RabbitMQ 3 - CQRS (MediatR 14) - FluentValidation - Mapster - Polly - BCrypt - JWT - Swagger / Scalar - Docker / Docker Compose

## PRERREQUISITOS

- Docker Desktop ≥ 4.x con Docker Compose V2

No se requiere instalar .NET SDK, SQL Server, Elasticsearch ni RabbitMQ — todo corre en contenedores.

## LEVANTAR EL PROYECTO

```bash
git clone <repositorio>
cd ultra-hotel-api
docker compose up -d
```

Esperar ~60 segundos a que los init containers completen. El API queda en `http://localhost:8080/swagger`.

Para limpiar todo (incluye volúmenes):

```bash
docker compose down -v
```

## SERVICIOS Y PUERTOS

| Servicio | Puerto | URL |
|---|---|---|
| API REST | 8080 | http://localhost:8080/swagger |
| SQL Server | 1433 | localhost,1433 |
| Elasticsearch | 9200 | http://localhost:9200 |
| RabbitMQ AMQP | 5672 | — |
| RabbitMQ UI | 15672 | http://localhost:15672 |

## VARIABLES DE ENTORNO

El archivo `.env` está incluido en el repositorio intencionalmente para que el evaluador pueda clonar y levantar sin configuración adicional. En producción estas credenciales irían en un gestor de secretos.

```env
SQL_SA_PASSWORD=Ultra_H0tel_2025
RABBITMQ_USER=ultra
RABBITMQ_PASS=Ultra_H0tel_2025
JWT_SECRET_KEY=UltraHotelProd_SuperSecretKey_MustBe32Chars!2025
```

## USUARIOS SEED

La base de datos se inicializa automáticamente con tres usuarios. **Contraseña de todos: `Ultra1234!`**

| Email | Rol | Permisos |
|---|---|---|
| admin@ultrahotel.com | ADMIN | Crear agentes |
| agent@ultrahotel.com | AGENT | Gestionar hoteles, habitaciones y ver reservas |
| traveler@ultrahotel.com | TRAVELER | Buscar habitaciones y crear reservas |

## POSTMAN

Importar `UltraHotel.postman_collection.json` desde la raíz del repositorio. Los tokens se guardan automáticamente como variables de colección y los IDs del seed vienen precargados. Ejecutar las carpetas en orden numérico (1 → 7).

## ARQUITECTURA

```
               WebApi                  ← punto de entrada HTTP, composition root
              /       \
             ▼         ▼
       Application  Infrastructure     ← Infrastructure implementa interfaces de Application
             \         /
              ▼       ▼
               Domain                  ← entidades y enums, sin dependencias externas

Commons  ←  contratos compartidos (IMessagePublisher, mensajes de integración)
Database/   scripts SQL + ES (fuera del grafo de dependencias .NET)
```

## DECISIONES DE ARQUITECTURA

**Clean Architecture** — las capas de negocio (Domain, Application) no dependen de frameworks ni infraestructura. Los handlers son testeables unitariamente sin SQL ni Elasticsearch. Cambiar SQL Server por PostgreSQL o ES por OpenSearch no toca la capa de Application.

**CQRS con MediatR** — Commands (escrituras) y Queries (lecturas) son clases separadas. El `ValidationBehavior<TRequest, TResponse>` ejecuta FluentValidation antes de cada handler, centralizando la validación sin repetir código.

**SQL Server + Elasticsearch (dual storage)** — SQL es la fuente de verdad con garantías ACID. Elasticsearch resuelve la búsqueda por ciudad y capacidad, que en SQL requeriría `LIKE` y joins costosos a escala. Cada escritura actualiza ambos; los fallos de ES son no fatales (Polly reintenta 3 veces con backoff exponencial antes de loguear).

**RabbitMQ para notificaciones** — al confirmar una reserva se publica un evento `BookingConfirmed`. El `BookingConfirmedConsumer` (BackgroundService) lo procesa en background con ack manual: si falla, el mensaje vuelve a la cola. La respuesta HTTP no espera el email.

**Polly** — retry con backoff exponencial en SQL Server, Elasticsearch y RabbitMQ. Evita fallos transitorios sin lógica manual de reintentos.

**Roles ADMIN / AGENT / TRAVELER** — `POST /auth/register` es público pero siempre crea TRAVELER (el campo `role` del body se ignora). `POST /auth/agents` requiere token ADMIN. Los ADMIN solo existen en el seed; no hay endpoint para crearlos desde fuera.

## REGLAS DE NEGOCIO

- Solo habitaciones con `room_enabled = true` y `hotel_enabled = true` aparecen en búsquedas.
- Al deshabilitar un hotel se re-indexan todas sus habitaciones en Elasticsearch con el nuevo flag.
- No se permite reservar si el rango de fechas se solapa con otra reserva de la misma habitación (409).
- El precio de la reserva se calcula al momento de crearla (noches × precio/noche).
- Los agentes no pueden crear otros agentes; solo el ADMIN puede hacerlo.

## USO DE IA EN EL DESARROLLO

**Herramienta:** Claude Code (claude-sonnet-4-6) vía extensión de VS Code, operando en modalidad multi-agente.

**Agentes involucrados:**

Claude Code permite delegar tareas a agentes especializados que corren en paralelo o en secuencia según la naturaleza del trabajo:

- **Agente de arquitectura (Plan)** — se usó en la fase de diseño para definir la estructura de capas, las responsabilidades de cada proyecto y las interfaces entre ellos. Las instrucciones fueron explícitas en cuanto a Clean Architecture, inversión de dependencias y que ninguna capa de negocio referenciara directamente un framework o infraestructura concreta.
- **Agente de exploración (Explore)** — utilizado para navegar el codebase, detectar inconsistencias entre interfaces e implementaciones y verificar que los `/// <inheritdoc />` estuvieran aplicados en todos los archivos de implementación antes de dar la tarea por completada.
- **Agente de código (claude / general-purpose)** — responsable de la generación de los handlers, repositorios, validators, configuración de Polly, Elasticsearch y RabbitMQ. Las instrucciones apuntaban a SOLID en cada clase: un handler = un caso de uso, un repositorio = un agregado, un behavior = una responsabilidad transversal.
- **Agente de revisión (code-reviewer)** — se aplicó para revisar decisiones puntuales, como la seguridad del endpoint de registro (detectó que aceptar `role` del body era un bug funcional) y la consistencia del pipeline de validación con el resto de los patrones del proyecto.

**Instrucciones clave que guiaron la generación:**

Las prompts no fueron genéricas: se especificó explícitamente que los handlers no podían acceder directamente a DbContext (solo a través de interfaces de repositorio), que la lógica de negocio debía vivir en el handler y no filtrarse al controller, que los controllers debían ser delgados (sin lógica condicional), y que cualquier validación de entrada debía declararse en un `AbstractValidator<T>` y no inline. Esto derivó en un código donde cada clase tiene una sola razón para cambiar y las dependencias fluyen siempre hacia adentro.

**Cómo se validó la salida:**

Cada bloque generado se revisó antes de aplicar. Se verificó coherencia entre interfaces y sus implementaciones, se levantó el stack completo con `docker compose up` y se ejecutaron los flujos end-to-end desde Postman. Los bugs detectados en esa validación (EF Core no traduce `string.Equals` con `StringComparison`, race condition en el init container de SQL Server) fueron diagnosticados y corregidos en la misma sesión sin intervención manual fuera del agente.

## NOTAS

- Rate limiting: 100 req/min por IP en general, 10 req/min en endpoints de auth.
- CORS configurable vía `Cors:AllowedOrigins` en `appsettings.json`.
