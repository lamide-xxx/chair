# IAM Policy that allows the worker to interact with SQS
resource "aws_iam_policy" "notifications_worker_policy" {
  name        = "${var.project_name}-notification-worker-policy"
  description = "Policy allowing worker to consume and delete SQS messages"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "sqs:ReceiveMessage",
          "sqs:DeleteMessage",
          "sqs:GetQueueAttributes",
          "sqs:GetQueueUrl"
        ]
        Effect   = "Allow"
        Resource = [
          aws_sqs_queue.chair_notifications_queue.arn,
          aws_sqs_queue.chair_notifications_queue_dlq.arn,
        ]
      },
    ]
  })
  
  tags = local.common_tags
}

# IAM Role for the worker service
resource "aws_iam_role" "notifications_worker_role" {
  name = "${var.project_name}-notification-worker-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      },
    ]
  })
  
    tags = local.common_tags
}

# Attach the policy to the role
resource "aws_iam_role_policy_attachment" "notifications_worker_policy_attach" {
  role       = aws_iam_role.notifications_worker_role.name
  policy_arn = aws_iam_policy.notifications_worker_policy.arn
}