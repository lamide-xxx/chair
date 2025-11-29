# ADR 007 – Use Redis for Distributed Caching

## Status
Accepted – 26 Nov 2025

## Context
Our `Chair.Api` service currently uses IMemoryCache, which only persists in-process.  
As the system scales horizontally across multiple instances, each instance maintains its own cache, causing redundant DB reads.

## Decision
We will migrate to Redis (via AWS ElastiCache) as a distributed cache backend.
Redis allows shared cache state across multiple API instances and survives restarts.

## Consequences
- ✅ Improves cache hit rate in multi-instance deployments
- ✅ Simplifies future horizontal scaling
- ⚠️ Requires additional infra (Terraform module + AWS cost)
- ⚠️ Slightly increased latency for cache reads vs in-memory

## Alternatives Considered
- Continue using IMemoryCache (simple but non-scalable)
- Use MemoryCache with background refresh (adds code complexity)
