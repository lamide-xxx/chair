resource "aws_sqs_queue" "chair_notifications_queue" {
  name                      = "${var.project_name}-notifications-queue"
  max_message_size          = 262144 # 1 MB
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.chair_notifications_queue_dlq.arn
    maxReceiveCount     = 3
  })

  lifecycle {
    ignore_changes = [max_message_size]
  }

  tags = local.common_tags
}

resource "aws_sqs_queue" "chair_notifications_queue_dlq" {
  name = "${var.project_name}-notifications-queue-dlq"
  max_message_size = 262144 # 1 MB

  lifecycle {
    ignore_changes = [max_message_size]
  }

  tags = local.common_tags
}

import {
  to = aws_sqs_queue.chair_notifications_queue
  id = "https://sqs.eu-west-2.amazonaws.com/568773573698/chair-notifications-queue"
}

import {
  to = aws_sqs_queue.chair_notifications_queue_dlq
  id = "https://sqs.eu-west-2.amazonaws.com/568773573698/chair-notifications-queue-dlq"
}