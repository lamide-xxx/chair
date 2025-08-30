# ADR-004: Authentication
Date: 2025-08-30  
Status: Accepted

## Context
We need secure auth, JWT-based integration with API, AWS-native if possible.

## Options
- **AWS Cognito (Chosen)**  
  Pros: AWS-native, integrates with API Gateway/IAM, lower cost.  
  Cons: Developer experience slightly rougher than Auth0.

- **Auth0**  
  Pros: Excellent DX, easy social login.  
  Cons: Expensive at scale.

- **Supabase Auth**  
  Pros: Simple.  
  Cons: Couples app too tightly with Supabase backend.

## Decision
Use **AWS Cognito** for authentication.

## Consequences
- ✅ AWS-native consistency.
- ✅ Low cost.
- ❌ Harder DX vs Auth0.  
