# Chair – Scalable Booking Platform

> **An event-driven booking system built with Next.js, .NET, AWS SQS, Terraform, and Grafana Cloud**  

---

## I built this to showcase distributed systems, observability, and infrastructure engineering in one cohesive project!

---

## Overview
Chair is a stylist discovery and booking platform demonstrating **real-world engineering principles**:
- Event-driven architecture (async booking pipeline via AWS SQS)
- Reliability patterns (DLQ, retries, idempotency)
- Observability (OpenTelemetry + Grafana Cloud)
- Infrastructure as Code (Terraform)
- CI/CD automation (GitHub Actions + Render)

---

## Architecture Snapshot

![System Diagram](docs/screenshots/architecture.png)

**Core Components**
| Component | Purpose |
|------------|----------|
| `Chair.Api` | ASP.NET Core API handling bookings and publishing SQS events |
| `Chair.NotificationsWorker` | Background service processing events and sending notifications |
| AWS SQS + DLQ | Decouples processing, adds resilience |
| Terraform | Automates provisioning of queues and roles |
| Grafana Cloud | Provides traces, metrics, and visual dashboards |
| Render + Vercel | Hosts backend and frontend respectively |

---

## Observability Highlights

![Grafana Dashboard](docs/screenshots/grafana.png)

- API & Worker traces stitched together with **W3C traceparent** headers
- Metrics: request latency, queue lag, failure count
- Grafana visualizes latency spikes, retries, DLQ growth

---

## Infra Automation

- Terraform provisions AWS resources with a **remote S3 state**.
- GitHub Actions pipeline automates:
    - Terraform plan & apply
    - API deployment to Render

---

## Engineering Decisions

- **Event-driven over synchronous HTTP**: decoupled booking creation from notification delivery via SQS, enabling independent scaling and fault isolation without tight service coupling.
- **Distributed tracing across service boundaries**: stitched API and worker traces together using W3C traceparent headers, giving full end-to-end visibility across async message flows, not just within a single service.
- **DLQ's, Retries & backoff**: rather than treating failure as an edge case, modelled failure as a first-class concern from the start. DLQs isolate poison messages, retries with backoff handle transient failures, and idempotency keys prevent duplicate processing.
- **IaC from day one**: provisioned all AWS resources via Terraform with remote S3 state, making infrastructure reproducible, reviewable, and deployable from CI without manual intervention
- **Load testing as a design validation tool**: used K6 to vallidate sub-50ms P99 latency under stress and confirm reliable worker recovery from failure, treating performance as a measurable property rather than an assumption

Full deep dive in [docs/architecture.md](docs/architecture.md).

---

##  Tech Stack

**Backend:** .NET 8, EF Core, Serilog, OpenTelemetry  
**Infra:** AWS SQS, Terraform, GitHub Actions, Render  
**Frontend:** Next.js (Vercel)  
**Monitoring:** Grafana Cloud

---

## Future Enhancements

- Redis caching for stylist listings
- ECS Fargate deployment for worker scalability
- Automated DLQ reprocessing
- Real email notifications (SES or SendGrid)

---

## Documentation
- [System Architecture](docs/architecture.md)
- [Lessons Learned](docs/lessons-learned.md)
- [Load Test Result](docs/screenshots/loadtest.png)

---

## Author
Built by Olamide Abegunde
> Building modern systems at the intersection of distributed infrastructure, cloud reliability, and applied AI.
