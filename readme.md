

# 🏨 WebHotel — Sistema de Gestión Hotelera

> Sistema web para la gestión integral de un hotel: reservas, habitaciones, usuarios y pagos en línea. Construido con ASP.NET Core MVC, Entity Framework Core y Stripe.

---

## 🚀 Demo rápida

> ⚠️ *En desarrollo activo — demo local disponible siguiendo las instrucciones de instalación.*

**Credenciales de prueba:**

| Rol | Email | Contraseña |
|---|---|---|
| Administrador | admin@webhotel.com | Admin123! |
| Empleado | empleado@webhotel.com | Empleado123! |
| Cliente | cliente@webhotel.com | Cliente123! |

---

## 🧩 ¿Qué problema resuelve?

WebHotel reemplaza la gestión manual de un hotel permitiendo que:

- **El administrador** controle habitaciones, usuarios y vea estadísticas en tiempo real desde un dashboard central.
- **Los empleados** gestionen disponibilidad y reservas del día a día.
- **Los clientes** busquen habitaciones disponibles, hagan su reserva y paguen en línea de forma segura.

---

## ✨ Características principales

### 🔐 Autenticación y autorización
- Login con **cookie-based authentication**
- **3 roles diferenciados**: Administrador, Empleado, Cliente
- Acceso restringido por rol en controladores y vistas
- Protección de rutas con `[Authorize(Roles = "...")]`
- **Expiración de sesión por inactividad** — la sesión se cierra automáticamente tras 15 minutos sin actividad (configurable)
- **Redirección automática al login** — cualquier intento de acceder a una ruta protegida con sesión expirada redirige al login sin exponer contenido

### 🏠 Gestión de habitaciones
- CRUD completo de habitaciones con imágenes
- Control de disponibilidad en tiempo real
- Almacenamiento de imágenes en servidor con referencia en BD

### 📅 Sistema de reservas
- Flujo completo: selección de fechas → confirmación → pago
- Reservas guardadas en base de datos con estado de pago
- Visualización de reservas por usuario

### 💳 Pagos con Stripe
- Integración real con **Stripe PaymentIntent API**
- Procesamiento seguro de tarjetas en modo test
- Registro de datos de pago no sensibles (últimos 4 dígitos, estado)

### 👥 Gestión de usuarios
- CRUD completo de usuarios por parte del administrador
- Asignación y modificación de roles
- Carga de imagen de perfil

### 📊 Dashboard con control de acceso por rol
- **Un solo dashboard** con visibilidad condicional según el rol del usuario
- El **Administrador** tiene acceso completo: usuarios, habitaciones, reservas e ingresos
- El **Empleado** ve únicamente las funcionalidades de su competencia
- Secciones y acciones ocultas/mostradas dinámicamente con lógica de autorización en vistas Razor
- Panel centralizado con AdminLTE

### 📄 Reportes PDF
- Generación de reportes con **QuestPDF**
- Exportación de información de reservas y ocupación
- Generado programáticamente desde código C# (sin plantillas externas)

---

## 🛠️ Stack tecnológico

| Categoría | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| Base de datos | SQL Server |
| Autenticación | Cookie Authentication |
| Pagos | Stripe API (PaymentIntent) |
| Frontend | Bootstrap 5, Font Awesome, AdminLTE |
| Arquitectura | Repository Pattern + Service Layer |
| Reportes PDF | QuestPDF |

---

## 🏗️ Arquitectura del proyecto

El proyecto sigue una separación de responsabilidades en capas dentro de una arquitectura monolítica:

```
WebHotel/
├── Controllers/        # Reciben requests HTTP, delegan lógica
├── Services/           # Lógica de negocio (reservas, pagos, usuarios)
├── Repositories/       # Acceso directo a base de datos via EF Core
├── Models/             # Entidades de dominio
├── DTOs/               # Objetos de transferencia de datos
├── Views/              # Vistas Razor por módulo
└── wwwroot/            # Archivos estáticos e imágenes subidas
```

**Decisiones técnicas:**
- **Repository Pattern** para desacoplar el acceso a datos de la lógica de negocio
- **Service Layer** para mantener los controladores delgados y la lógica centralizada
- **Cookie Authentication** sobre JWT porque es una app MVC server-side (estado manejado en servidor)
- **Stripe PaymentIntent** sobre Checkout para mayor control del flujo de pago

---

## ⚙️ Instalación local

### Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) o SQL Server Express
- [Cuenta Stripe](https://stripe.com) (modo test, gratis)

### Pasos

**1. Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/WebHotel.git
cd WebHotel
```

**2. Configurar variables de entorno**

Edita `appsettings.json` o crea `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebHotelDB;Trusted_Connection=True;"
  },
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_..."
  }
}
```

**3. Aplicar migraciones**
```bash
dotnet ef database update
```

**4. Ejecutar el proyecto**
```bash
dotnet run
```

Abre `https://localhost:5001` en tu navegador.

---

## 🗄️ Modelo de base de datos

Entidades principales:

- `Usuario` — datos de cuenta, rol asignado, imagen de perfil
- `Habitacion` — número, tipo, precio por noche, disponibilidad, imagen
- `Reserva` — fechas, usuario, habitación, estado (Pendiente / Pagada / Cancelada)
- `Pago` — referencia a reserva, últimos 4 dígitos, monto, fecha, estado Stripe

---

## 🔄 Estado del proyecto

| Módulo | Estado |
|---|---|
| Autenticación y roles | ✅ Completo |
| Gestión de habitaciones | ✅ Completo |
| Gestión de usuarios | ✅ Completo |
| Dashboard con control por rol | ✅ Completo |
| Carga de imágenes | ✅ Completo |
| Reportes PDF con QuestPDF | ✅ Completo |
| Sistema de reservas | 🔄 En progreso |
| Integración con Stripe | 🔄 En progreso |
| Notificaciones automáticas | 📋 Planeado |

---

## 📸 Capturas de pantalla

> *Próximamente — capturas del dashboard, flujo de reserva y pago.*

---

## 👨‍💻 Autor

**Ronald Retana** — Desarrollador Backend  
📍 Pérez Zeledón, Costa Rica  
🔗 [LinkedIn](www.linkedin.com/in/ronaldretana) · [GitHub](https://github.com/rretana86-max)

---

## 📄 Licencia

Este proyecto es de uso educativo y de portfolio personal.
