# Chair Infrastructure (Terraform)

This folder defines the cloud infrastructure for the Chair platform.

## Resources
- AWS SQS main queue (`chair-queue`)
- AWS SQS dead-letter queue (`chair-dlq`)

## How to deploy
```bash
cd infra
terraform init
terraform plan
terraform apply
```

## Variables

- `aws_region`: Deployment region (default eu-west-2)
- `project_name`: Project resource prefix

## Outputs
- `notifications_queue_url`
- `notifications_queue_dlq_url`

