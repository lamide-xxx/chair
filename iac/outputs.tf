output "notifications_queue_url" {
  description = "The URL of the main SQS queue"
  value       = aws_sqs_queue.chair_notifications_queue.id
}

output "notifications_queue_dlq_url" {
  description = "The URL of the dead letter queue"
  value       = aws_sqs_queue.chair_notifications_queue_dlq.id
}

