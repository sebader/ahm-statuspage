# AHM Status Page

A status page application that displays service health information from Azure Health Models (AHM). Built with React and Azure Functions, deployed using Azure Static Web Apps.

## Project Structure

```
.
├── api/                    # Azure Functions API backend (C#)
│   ├── Models/            # Data models for health responses
│   ├── Services/          # Services for interacting with AHM
│   ├── Utils/             # Utility functions
│   └── sample_data/       # Sample response data for testing
├── app/                    # React frontend application
│   ├── src/
│   │   ├── components/    # React components
│   │   ├── config/        # Frontend configuration
│   │   └── types/         # TypeScript type definitions
│   └── public/            # Static assets
```

## Features

- Real-time service health status display
- Component relationship visualization
- Historical status timeline
- Customizable theming and branding
- Integration with Azure Health Models

## Configuration

### Frontend Configuration (app/src/config/app-config.json)

```json
{
  "appName": "Your App Name",
  "companyName": "Your Company",
  "pageHeader": "Custom Header",
  "pageSubheader": "Current status of our services",
  "theme": {
    "primaryColor": "#2c3e50",
    "backgroundColor": "#f5f6fa",
    "fontFamily": "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif"
  }
}
```

### Backend Configuration (api/local.settings.json)

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "EntityNames": "comma,separated,entity,names",
    "HealthModelsHost": "https://< your health model endpoint>.healthmodels.azure.com",
    "IsLocalEnvironment": "true/false"
  }
}
```

## Setup

1. **Prerequisites**
   - Node.js and npm
   - .NET 8.0 SDK
   - Azure Functions Core Tools
   - Azure Static Web Apps CLI

2. **Frontend Setup**
   ```bash
   cd app
   npm install
   npm start
   ```

3. **Backend Setup**
   ```bash
   cd api
   dotnet restore
   func start
   ```

4. **Configuration**
   - Update `app/src/config/app-config.json` with your branding
   - Create `api/local.settings.json` with your AHM configuration for running the Function app locally
   - Configure entity IDs in the backend settings

## Deployment

The application is designed to be deployed to Azure Static Web Apps with an Azure Functions backend. The repository includes GitHub Actions workflows for automated deployment.

1. **Backend API**: Deploys to Azure Functions
2. **Frontend**: Deploys to Azure Static Web Apps

## Local Development

### Prerequisites
- Node.js and npm
- .NET 8.0 SDK
- Azure Functions Core Tools
- Azure Static Web Apps CLI (`npm install -g @azure/static-web-apps-cli`)

### Running Locally with Static Web Apps CLI

1. **Run with Static Web Apps CLI**
   ```bash
   # In the root directory
   swa start --api-location api --run "npm run build"
   ```

This will start the Static Web Apps CLI, serving your application at `http://localhost:4280`. The CLI provides a local development environment that closely mirrors the Azure Static Web Apps production environment.

## Azure Deployment

### 1. Azure Resources Setup

1. **Create an Azure Function App**
   - Create a new Function App in the Azure Portal
   - Runtime stack: .NET 8 (LTS)
   - Operating System: Windows or Linux
   - Hosting: Plan type according to your needs

2. **Configure Function App Settings**
   Add the following application settings in the Azure Portal:
   ```
   AzureWebJobsStorage         : Your storage account connection string
   FUNCTIONS_WORKER_RUNTIME    : dotnet-isolated
   EntityNames                 : Your comma-separated entity names
   HealthModelsHost           : Your AHM endpoint
   IsLocalEnvironment         : false
   ```

3. **Create an Azure Static Web App**
   - Create a new Static Web App in the Azure Portal
   - SKU: Standard (required for custom Functions integration)
   - Region: Choose based on your needs
   - Source: Select your GitHub repository

### 2. Link Function App to Static Web App

1. Navigate to your Static Web App in the Azure Portal
2. Go to "API" section
3. Link your Function App as the backend API
4. Update the GitHub Actions workflow files if needed

### 3. Deploy

The repository includes GitHub Actions workflows for automated deployment:
- `.github/workflows/azure-static-web-apps-*.yml` for the Static Web App
- `.github/workflows/azure-function-app_*.yml` for the Function App

Simply push to your main branch to trigger deployment.

## Architecture

The application follows a client-server architecture:

1. **Frontend (React)**
   - Displays service health status
   - Updates in real-time
   - Configurable theming and branding

2. **Backend (Azure Functions)**
   - Integrates with Azure Health Models
   - Provides REST API endpoints
   - Handles data transformation and caching

3. **Azure Health Models**
   - Source of health status data
   - Provides entity relationships
   - Maintains health state history
