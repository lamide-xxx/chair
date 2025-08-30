# ADR-002: Backend Compute
Date: 2025-08-30  
Status: Accepted

## Context
Backend is .NET 8 Web API. MVP needs cost-efficiency, scalability, minimal infra ops.

## Options
- **AWS Lambda + API Gateway (Chosen)**  
  Pros: Pay-per-use, scales to zero, minimal ops.  
  Cons: Cold start penalty, limited execution time.

- **ECS Fargate**  
  Pros: Good for high/constant workloads.  
  Cons: Higher idle cost, more setup.

- **EC2**  
  Pros: Full control.  
  Cons: Too heavy, manual scaling, not serverless.

## Decision
Use **AWS Lambda + API Gateway** for backend API.

## Consequences
- ✅ Cheap (pay-per-use).
- ✅ Scales automatically.
- ❌ Cold start penalty.
- 🚀 Migration path: ECS Fargate if traffic grows consistently.  
