**#  System Architecture for Chair

**Chair** is a stylist discovery and booking platform built to demonstrate real-world, production-grade engineering skills across backend, distributed systems, event-driven architecture, observability, and cloud deployment.

This document outlines the system architecture, scaling strategy, and the reliability patterns implemented.

---
### 1. High-Level Architecture
Frontend (Next.js, Vercel)
|
v
Backend API (ASP.NET Core, Render)
|
v
PostgreSQL (Neon)
|
v
AWS SQS (Event Queue)
|
v
Notifications Worker (.NET Worker Service)
|
v
Email / Push (Future)


### Key Concepts
- The API handles user requests and writes bookings.
- Booking confirmation events are published asynchronously to SQS.
- A background worker consumes messages, processes notifications, and handles retries or DLQ.
- Distributed tracing ties together API → Queue → Worker for full visibility end-to-end.

---

## Backend API (`Chair.Api`)

### Technologies
- **.NET 8 Web API**
- **EF Core + PostgreSQL (Neon)**
- **OpenTelemetry (Tracing + Metrics)**
- **Serilog structured logging**
- **Deployed on Render**

### Responsibilities
- CRUD for users, stylists, services, and appointments.
- Publishes booking events to SQS.
- Exposes REST endpoints consumed by the Next.js frontend.

### Observability
The API is fully instrumented using:
- **AspNetCore instrumentation** – incoming HTTP requests
- **HttpClient instrumentation**
- **EF Core instrumentation**
- **OTLP exporter → Grafana Cloud**
- **Structured logs → Serilog**

**Why:**  
This provides real-time visibility into:
- Request latency (p50/p95/p99)
- Database performance
- API error rates
- End-to-end traces that propagate into the worker

### AI-Powered Stylist Recommendations
Integrated OpenAI API to enhance stylist discovery:
- User describes desired hairstyle → backend generates recommended stylists based on service tags.
- Runs as a separate microservice endpoint integrated into the stylist search flow.
- Demonstrates **applied AI** integration into distributed architecture.


---

## Event-Driven Booking Pipeline

Bookings are processed asynchronously to improve performance, responsiveness, and reliability.

### Flow
1. User creates an appointment via UI.
2. API writes appointment to DB.
3. API publishes an event: `BOOKING_CREATED` into **AWS SQS**.
4. Worker consumes event and sends the notification.
5. Worker deletes message on successful completion.
6. Failed messages go through retry policy → then DLQ.

### Why Event-Driven?
- Decouples API performance from notification processing.
- Prevents slowdowns when many bookings happen simultaneously.
- Enables future features like:
    - Fanout notifications
    - Analytics services
    - Payment processing
    - Stylist availability updates

This architecture allows **scalability without rewriting the system**.

---

## Notifications Worker (`Chair.NotificationsWorker`)

### Technologies
- .NET BackgroundService
- AWS SQS
- Polly (retries + exponential backoff)
- OpenTelemetry (manual spans + custom metrics)
- Serilog

### Responsibilities
- Poll SQS for booking events.
- Process notifications (placeholder logic).
- Retry transient failures automatically.
- Push unprocessed messages to DLQ.

### Reliability Features

#### ✔ Exponential Backoff with Polly
```csharp
1s → 2s → 4s (retry)
```

Prevents retry storms during transient network issues.

#### ✔ Dead Letter Queue (DLQ)

Messages that fail after retries are automatically routed to DLQ.

#### ✔ Idempotent Handlers

Worker ensures duplicate messages (SQS can deliver >1) don’t double-send notifications.

#### ✔ Rate Limiting
- Integrated middleware to enforce per-IP request throttling.
- Protects backend endpoints from burst loads and abuse.
- Returns `429 Too Many Requests` when exceeded — verified through Postman burst testing.

## Distributed Tracing (API → SQS → Worker)

### Implemented With
- W3C **traceparent** propagation via SQS message attributes
- Manual `ActivitySource` spans in the worker
- Auto-instrumentation in the API

### Result

A single trace in Grafana shows:

    POST /api/appointments
        ├── Insert into DB
        ├── PublishToSqs
        └── ProcessBookingMessage (Worker)
        └── SendNotification


### This Enables
- Cross-service correlation
- Root cause debugging

---

## Custom Metrics (Prometheus → Grafana)

### API Metrics
- Request latency (avg, p95)
- Request throughput
- Error rate

### Worker Metrics
- Queue lag (`sqs_queue_lag_ms`)
- Processing time (`notification_processing_time_ms`)
- Failure counts (`notification_failures`)

### Dashboard Panels

| **Panel** | **Metric** | **Description** |
|------------|-------------|-----------------|
| API Latency | `http_server_request_duration_sum / count` | Request latency over time |
| Worker Time | `sqs_notification_processing_time_ms` | Average notification processing duration |
| Queue Lag | `sqs_queue_lag_ms` | Time spent in queue before processing |
| Failures | `notification_failures` | Failed notifications per interval |
| Trace List | Tempo datasource | End-to-end trace view |

---

## Frontend (Next.js + Vercel)

The frontend uses:
- Client-side fetching from Chair API
- Secure CORS configuration
- Clean UX for browsing stylists + booking appointments

### Deployed on Vercel for:
- Instant global CDN caching
- Seamless builds
- Zero infrastructure overhead

---

## Cloud Infrastructure

### Provider Mix

| **Component** | **Provider** |
|----------------|--------------|
| Frontend | Vercel |
| Backend API | Render |
| Database | Neon |
| Queue | AWS SQS |
| Observability | Grafana |

### Design Goals
Each component was chosen intentionally to:
- Minimize cost
- Maximize reliability
- Provide a modern cloud experience without vendor lock-in

### Infrastructure as Code (Terraform)
All AWS resources (SQS, DLQ, IAM roles) are provisioned through Terraform, with:
- Remote S3 backend for state management
- Least-privilege IAM policies for worker access
- Tagged, version-controlled configuration for auditable infra

This ensures the system can be re-created identically across environments — a prerequisite for real scalability.

---

## Scalability Features

- API can be **horizontally scaled** (stateless).
- SQS naturally **buffers spikes** in bookings.
- Worker can **scale to multiple consumers**.
- DLQ prevents **systemic failures**.
- OpenTelemetry metrics enable **data-driven autoscaling** decisions.


## Load Testing & Resilience Validation
Used **k6** to simulate 20 concurrent users hitting `/api/appointments` for 60s:
- API sustained throughput with <2% error rate.
- Queue lag and retry rates observed in Grafana.
- Worker manually killed mid-run — DLQ captured unprocessed events as designed.

Result: The system **recovered gracefully**, confirming reliability of async design.
---

## Blue/Green Deployments
- Render Preview Environments are now used to simulate Blue/Green deploys.  
- Each deployment spins up a new environment, performs a `/healthz` probe, and promotes only if healthy.  
- This enables **zero-downtime releases** and quick rollbacks for safer iteration.

---
##  Security & HTTPS
Added:
- `UseHttpsRedirection()`
- Secure headers (`X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy`, etc.)
  to harden the API against common web attacks.

This elevates the app closer to **production-grade security posture**.

##  Future Infra Roadmap
- Redis caching (stylist search performance).
- Kafka or SNS/SQS fanout.
- Payment service decoupling.
- Real email/SMS notifications.
