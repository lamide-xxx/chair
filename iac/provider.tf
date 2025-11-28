terraform {
  backend "s3" {
    bucket = "chair-terraform-state"      # S3 bucket to store the Terraform state
    key    = "chair/terraform.tfstate" # Path within the bucket for the state file
    region = "eu-west-2"                  # AWS region where the S3 bucket is located
    encrypt = true                        # Enable server-side encryption for the state file
    # profile = "personal"               # Use the 'personal' AWS CLI profile for authentication
  }
  
  required_providers {
    aws = {
      source  = "hashicorp/aws" # Specify the source of the AWS provider
      version = "~> 5.0"        # Use a version of the AWS provider that is compatible with version
    }
  }
  required_version = ">= 1.4.0" # Ensure that the Terraform version is 1.0.0 or higher
}

provider "aws" {
  region = var.aws_region # Set the AWS region using a variable
  # profile = "personal"  # Use the 'personal' AWS CLI profile for authentication
}
