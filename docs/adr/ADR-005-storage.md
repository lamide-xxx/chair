# ADR-005: Storage
Date: 2025-08-30  
Status: Accepted

## Context
We need scalable, cheap, durable object storage for user-uploaded files (images, docs, etc).

## Options
- **AWS S3 (Chosen)**  
  Pros: Industry standard, cheap, integrates with Cognito/IAM.  
  Cons: Slightly more infra setup.

- **Supabase Storage**  
  Pros: Simple.  
  Cons: Vendor lock-in.

- **Local Disk**  
  Cons: Not viable for serverless infra.

## Decision
Use **AWS S3** for storage.

## Consequences
- ✅ Cheap, scalable, durable.
- ✅ Industry standard.
- 🚀 Integrates seamlessly with Cognito/IAM for secure uploads.  
