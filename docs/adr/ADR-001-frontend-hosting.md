# ADR-001: Frontend Hosting
Date: 2025-08-30  
Status: Accepted

## Context
Frontend is a Next.js web app (PWA). Needs global delivery, HTTPS, CI/CD, and low cost.

## Options
- **AWS S3 + CloudFront**  
  Pros: More control, AWS-native, production-ready.  
  Cons: Requires more infra setup for MVP.

- **Vercel (Chosen)**  
  Pros: Fast iteration, free tier covers MVP, seamless Next.js integration.  
  Cons: Less AWS-native, migration needed later for AWS consistency.

- **Lambda**  
  Pros: Compute-oriented.  
  Cons: Not suitable for hosting static assets / frontend.

## Decision
Host frontend with **Vercel** for MVP.

## Consequences
- ✅ Fast iteration speed.
- ✅ Low cost (free tier).
- ❌ Will likely migrate to S3 + CloudFront later for AWS consistency.  
