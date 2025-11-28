locals {
  common_tags = {
    Environment = "production"
    Project     = var.project_name
    ManagedBy   = "terraform"
  }
}
