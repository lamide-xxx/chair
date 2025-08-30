# ADR-006: Observability
Date: 2025-08-30  
Status: Accepted

## Context
We need structured logging, metrics, and traces for debugging. Must be cheap but extensible.

## Options
- **Serilog + OpenTelemetry → CloudWatch/X-Ray (Chosen)**  
  Pros: Low cost, AWS-native, extensible.  
  Cons: Requires some setup.

- **Datadog / New Relic**  
  Pros: Excellent dashboards.  
  Cons: Expensive as scale grows.

- **Self-hosted ELK**  
  Pros: Full control.  
  Cons: Too heavy for MVP.

## Decision
Use **Serilog + OpenTelemetry** with CloudWatch/X-Ray.

## Consequences
- ✅ Cheap and AWS-native.
- ✅ Extensible.
- ❌ Less polished than Datadog at scale.  
