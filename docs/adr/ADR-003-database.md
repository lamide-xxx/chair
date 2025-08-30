# ADR-003: Database
Date: 2025-08-30  
Status: Accepted

## Context
We need relational DB with low cost, strong SQL support, and serverless scaling.

## Options
- **Neon Serverless Postgres (Chosen)**  
  Pros: Free/cheap tier, Postgres-compatible, serverless scaling.  
  Cons: Newer service, not AWS-native.

- **AWS RDS Postgres**  
  Pros: AWS-native, production-grade.  
  Cons: Higher baseline cost.

- **Supabase**  
  Pros: Built-in auth/storage/APIs.  
  Cons: Redundant with our backend + Cognito.

## Decision
Use **Neon Serverless Postgres** for MVP.

## Consequences
- ✅ Cheap/free hosting.
- ✅ Postgres portability.
- 🚀 Easy migration path to RDS later.  
