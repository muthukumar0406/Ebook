# Ebook Hub - Run Guide

This guide describes how to run the Ebook Hub application (Backend API + Angular Frontend).

## Prerequisites

- **Docker** (Desktop or Engine)
- **.NET 8 SDK** (for local dev)
- **Node.js 20+** (for local dev)
- **SQL Server** (or use the one in docker-compose if configured, currently configured to expect external or local SQL Express)

## Configuration (Critical)

> [!IMPORTANT]
> **Before running the application, you must update the configuration files with your real Firebase credentials.**

1.  **Frontend**: Open `ebook-hub-ui/src/environments/environment.ts` and replace the placeholder values with your Firebase config object.
2.  **Backend**: Open `EbookHub.API/appsettings.json` and set the correct `ProjectId` under the `Firebase` section.

## Option 1: Run with Docker (Recommended)

This method runs both the API and the UI in containers.

1. **Navigate to the root directory** (where `docker-compose.yml` is located).
2. **Build and start the services**:
   ```bash
   docker-compose up --build
   ```
   This will start:
   - **API**: localhost:5000
   - **UI**: localhost:4200
   - **SQL Server**: localhost:1433 (mapped)

3. **Access the application**:
   - Frontend: [http://localhost:4200](http://localhost:4200)
   - Backend Swagger: [http://localhost:5000/swagger](http://localhost:5000/swagger)

> **Note**: The database is automatically provisioned. If the API fails to connect locally initially, it might be waiting for the DB to initialize. Docker Compose `depends_on` starts the container but doesn't wait for SQL to be ready for connections. Give it a moment or restart the API container if needed.

## Option 2: Run Locally (Manual)

### Backend (API)

1. Navigate to `EbookHub.API`:
   ```bash
   cd EbookHub.API
   ```
2. Restore and Run:
   ```bash
   dotnet restore
   dotnet run
   ```
   The API will start on ports defined in `launchSettings.json` (usually 5000-5200).

### Frontend (UI)

1. Navigate to `ebook-hub-ui`:
   ```bash
   cd ebook-hub-ui
   ```
2. Install dependencies (if not already done):
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm start
   ```
4. Open [http://localhost:4200](http://localhost:4200) in your browser.

## Troubleshooting

- **Database Connection**: Ensure the SQL connection string in `appsettings.json` is correct. For Docker, you might need to use `Server=host.docker.internal;...` to access a DB on your host machine.
- **Ports**: If ports 4200 or 5000 are in use, modify `docker-compose.yml` accordingly.
