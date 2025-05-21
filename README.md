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
    "EntityNames": "comma,separated,entity,ids",
    "HealthModelsHost": "your-health-models-host",
    "IsLocalEnvironment": "true"
  }
}
```

## Setup

1. **Prerequisites**
   - Node.js and npm
   - .NET 6.0 SDK
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
   - Create `api/local.settings.json` with your AHM configuration
   - Configure entity IDs in the backend settings

## Deployment

The application is designed to be deployed to Azure Static Web Apps with an Azure Functions backend. The repository includes GitHub Actions workflows for automated deployment.

1. **Backend API**: Deploys to Azure Functions
2. **Frontend**: Deploys to Azure Static Web Apps

## Development

- The frontend runs on `http://localhost:3000` by default
- The backend API runs on `http://localhost:7071`
- Use `swa start` for local testing with the Static Web Apps CLI

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
