#!/bin/bash

# Azure Container Apps Deployment Script for HRAgent
# This script deploys the infrastructure and applications to Azure

set -e  # Exit on error

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Default values
ENVIRONMENT="${1:-dev}"
RESOURCE_GROUP="rg-hragent-${ENVIRONMENT}"
LOCATION="${2:-westus2}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    log_error "Azure CLI is not installed. Please install it first."
    exit 1
fi

# Check if logged in to Azure
if ! az account show &> /dev/null; then
    log_error "Not logged in to Azure. Please run 'az login' first."
    exit 1
fi

# Validate environment parameter
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|prod)$ ]]; then
    log_error "Invalid environment: $ENVIRONMENT. Must be dev, staging, or prod."
    exit 1
fi

log_info "Deploying HRAgent to Azure Container Apps"
log_info "Environment: $ENVIRONMENT"
log_info "Resource Group: $RESOURCE_GROUP"
log_info "Location: $LOCATION"

# Create resource group if it doesn't exist
log_info "Creating resource group..."
az group create \
    --name "$RESOURCE_GROUP" \
    --location "$LOCATION" \
    --output none

log_info "Resource group created/verified"

# Validate Bicep template
log_info "Validating Bicep template..."
VALIDATION_RESULT=$(az deployment group validate \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$SCRIPT_DIR/main.bicep" \
    --parameters "$SCRIPT_DIR/parameters.${ENVIRONMENT}.json" \
    --output json)

if [ $? -ne 0 ]; then
    log_error "Bicep template validation failed"
    echo "$VALIDATION_RESULT"
    exit 1
fi

log_info "Bicep template validation passed"

# Run what-if to preview changes
log_info "Running what-if analysis..."
az deployment group what-if \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$SCRIPT_DIR/main.bicep" \
    --parameters "$SCRIPT_DIR/parameters.${ENVIRONMENT}.json"

# Prompt for confirmation
read -p "Do you want to proceed with deployment? (yes/no): " CONFIRM
if [ "$CONFIRM" != "yes" ]; then
    log_warn "Deployment cancelled by user"
    exit 0
fi

# Deploy infrastructure
log_info "Deploying infrastructure..."
DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$SCRIPT_DIR/main.bicep" \
    --parameters "$SCRIPT_DIR/parameters.${ENVIRONMENT}.json" \
    --output json)

if [ $? -ne 0 ]; then
    log_error "Deployment failed"
    echo "$DEPLOYMENT_OUTPUT"
    exit 1
fi

log_info "Deployment completed successfully"

# Extract outputs
API_URL=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.apiUrl.value')
UI_URL=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.properties.outputs.uiUrl.value')

log_info "Deployment outputs:"
log_info "  API URL: $API_URL"
log_info "  UI URL: $UI_URL"

# Verify health endpoints
log_info "Verifying health endpoints..."
sleep 10  # Wait for containers to start

if curl -s -f "${API_URL}/health" > /dev/null; then
    log_info "API health check: PASSED"
else
    log_warn "API health check: FAILED (may need more time to start)"
fi

if curl -s -f "${UI_URL}/health" > /dev/null; then
    log_info "UI health check: PASSED"
else
    log_warn "UI health check: FAILED (may need more time to start)"
fi

log_info "Deployment complete! 🎉"
log_info "Access the application at: $UI_URL"
