variable "aws_region" {
    type        = string
    default     = "eu-west-2"                  
    description = "The AWS region to deploy resources in"
}

variable "project_name" {
    type        = string
    default     = "chair"                  
    description = "The project name to be used to prefix resources with"
}