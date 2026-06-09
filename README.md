# 🛒 ECommerce Microservices Solution

> A high-performance, distributed e-commerce backend built with **.NET 9**, demonstrating **Microservices Architecture**, inter-service communication via binary **gRPC**, and hybrid client interaction utilizing **gRPC-JSON Transcoding**.

---

## 🏗️ Solution Overview

The ecosystem consists of three specialized microservices working together to process orders:

| Service | Role | Protocol |
|---|---|---|
| **OrderService** | Gateway / Orchestrator | Hybrid (REST + gRPC) |
| **InventoryService** | Stock management & validation | Pure gRPC (internal) |
| **PaymentService** | Transactional balance handling | Pure gRPC (internal) |

```
                        ┌──────────────────────────────┐
External Clients  ───▶  │         OrderService          │
  REST / gRPC           │   (Hybrid Facade + gRPC-JSON  │
                        │         Transcoding)           │
                        └───────────┬──────────┬────────┘
                                    │          │
                              gRPC  │          │  gRPC
                                    ▼          ▼
                        ┌───────────────┐  ┌─────────────────┐
                        │InventoryService│  │  PaymentService  │
                        │  (Pure gRPC)  │  │   (Pure gRPC)   │
                        └───────────────┘  └─────────────────┘
```

### Service Descriptions

**🔀 OrderService — The Gateway/Orchestrator**
Acts as a hybrid facade exposing dual-protocol endpoints — accepting both **REST (JSON/HTTP/1.1)** and **gRPC (Proto/HTTP/2)**. It orchestrates all downstream calls to the inventory and payment processors.

**📦 InventoryService**
A high-performance, internal-only pure gRPC service responsible for managing stock levels and validating product deductions.

**💳 PaymentService**
An internal-only pure gRPC service handling transactional amounts and balance verification.

---

## ⚡ Key Architectural Feature: gRPC-JSON Transcoding

Instead of building a separate API Gateway or duplicating controllers, the **OrderService** leverages native **gRPC-JSON Transcoding**.

By annotating the `.proto` service definition, the service dynamically generates a reverse proxy middleware that maps incoming HTTP REST requests straight into binary gRPC service implementations — **one implementation, two protocols**.

### Protocol Routing Mechanics

```
┌─────────────────────┐     HTTP/1.1 REST JSON      ┌──────────────────┐
│  Frontend / Web App  │ ─────────────────────────▶  │                  │
└─────────────────────┘                              │   OrderService   │
                                                     │  :5070 / :7150   │
┌─────────────────────┐     HTTP/2 TLS + gRPC        │                  │
│  Postman gRPC Client │ ─────────────────────────▶  │                  │
└─────────────────────┘   (Server Reflection)        └────────┬─────────┘
                                                              │
                                              Binary gRPC RPC │ (HTTP/2)
                                                              │
                             ┌────────────────────────────────┴──────────────────────────────┐
                             │                                                                │
                             ▼                                                                ▼
                  ┌──────────────────────┐                                      ┌──────────────────────┐
                  │   InventoryService   │                                      │    PaymentService    │
                  │       :5144          │                                      │       :5071          │
                  └──────────────────────┘                                      └──────────────────────┘
```

| Client Type | Transport | Format |
|---|---|---|
| External Clients (Frontend/Web) | HTTP/1.1 | REST JSON |
| External Testing Tools (Postman gRPC) | HTTP/2 TLS | Binary Proto |
| Internal Service-to-Service | HTTP/2 | Binary gRPC |

---

## 🛠️ Technology Stack

| Category | Technology |
|---|---|
| **Runtime** | .NET 9 SDK |
| **Protocols** | gRPC (HTTP/2), REST (HTTP/1.1 via Transcoding) |
| **API Documentation** | OpenAPI / Swagger |
| **Serialization** | Protocol Buffers (Proto3), JSON |

---

## 🚀 Getting Started & Local Setup

### 1. Prerequisites

Ensure you have the latest [**.NET 9 SDK**](https://dotnet.microsoft.com/download/dotnet/9.0) installed.

```bash
dotnet --version
# Should output 9.x.x
```

### 2. Running the Solution

Navigate to the **solution root folder** and run all microservices simultaneously:

```bash
dotnet run --launch-profile http
```

> **Visual Studio users:** Right-click the solution → *Configure Startup Projects* → Set all 3 projects to **Start**.

### 3. Port Allocation

| Service | Protocol Capability | HTTP Port | HTTPS Port |
|---|---|---|---|
| **OrderService** | Hybrid (REST + gRPC) | `5070` | `7150` |
| **InventoryService** | Pure gRPC (HTTP/2 only) | `5144` | Internal |
| **PaymentService** | Pure gRPC (HTTP/2 only) | `5071` | Internal |

---

## 🧪 Testing the APIs

### Option A — Web REST UI Tester (`index.html`)

A lightweight, pre-configured static HTML testing utility is included in the root folder.

1. Ensure the solution is running
2. Open `index.html` directly in any web browser
3. Target `http://localhost:5070` or `https://localhost:7150`
4. Trigger any of the preloaded actions:

| Action | Method | Endpoint |
|---|---|---|
| Create an Order | `POST` | `/api/orders` |
| Fetch a specific Order | `GET` | `/api/orders/{id}` |
| Fetch all Orders | `GET` | `/api/orders` |

---

### Option B — Native gRPC Testing via Postman

Because **Server Reflection** (`AddGrpcReflection()`) is enabled in development on the OrderService, you do **not** need to import `.proto` files manually.

1. Open Postman → **New** → **gRPC Request**
2. Enter the secure channel URL:
   ```
   https://localhost:7150
   ```
3. Under **Service Definition**, select **Use gRPC Reflection**
4. Click 🔄 — methods (`CreateOrder`, `GetOrder`, etc.) will auto-populate instantly

---

## 🔒 CORS Configuration

The OrderService includes a global **AllowAll** CORS policy configured in `Program.cs`. This ensures local cross-origin browser requests (e.g. `fetch` calls from a `file://` protocol page) pass preflight checks without issues.

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

app.UseCors("AllowAll"); // Must be placed before UseAuthorization
```

---

## 📁 Project Structure

```
/
├── OrderService/
│   ├── Protos/               # .proto definitions with HTTP transcoding annotations
│   ├── Services/             # gRPC service implementations
│   ├── Controllers/          # (optional REST controllers)
│   ├── Program.cs
│   └── appsettings.json
├── InventoryService/
│   ├── Protos/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
├── PaymentService/
│   ├── Protos/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
├── index.html                # Web REST UI Tester
└── ECommerce.sln
```
