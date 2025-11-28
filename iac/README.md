# Chair Infrastructure (Terraform)

This folder defines and manages the cloud infrastructure for the **Chair** platform using **Terraform**.  
All infrastructure is version-controlled and state-managed via a remote S3 backend.

---

## Resources

| Resource | Description |
|-----------|--------------|
| **AWS SQS Queue** (`chair-notifications-queue`) | Main message queue for asynchronous booking and notification events. |
| **AWS SQS DLQ** (`chair-notifications-queue-dlq`) | Dead Letter Queue for messages that fail after multiple retries, improving reliability. |
| **IAM Policy** (`chair-notifications-worker-policy`) | Grants the worker least-privilege access to receive and delete SQS messages. |
| **IAM Role** (`chair-notifications-worker-role`) | Execution role assumed by ECS Fargate tasks running the `Chair.NotificationsWorker` service. |
| **S3 Remote Backend** | Stores Terraform state remotely for shared, consistent infrastructure management. |

---

## Design Decisions

- **ECS Fargate over Lambda**:  
  Chosen for the **long-running, continuous nature** of the worker service.  
  Fargate provides better control over containerized workloads, concurrency, and resource scaling,  
  while Lambda is optimized for short-lived, event-triggered executions.  
  The worker continuously polls SQS, making ECS a more appropriate compute model.

- **Dead Letter Queue (DLQ)**:  
  Ensures resilience by isolating unprocessed events for manual inspection or reprocessing.

- **Least Privilege IAM**:  
  The worker role is restricted to only the actions and queues it needs, enhancing security posture.

---

## 🚀 How to Deploy

```bash
cd infra
terraform init
terraform plan
terraform apply
