# Hangfire Worker Service

A .NET background service application using Hangfire that reads data from a PostgreSQL database every 1 minute, then prints that data to the console after a 2-second delay.

## Prerequisites

- .NET 10 SDK
- PostgreSQL database

## Setup

1. Create a PostgreSQL database named `hangfire` (or update the connection string in `appsettings.json`)
2. Update the connection string in `appsettings.json` with your PostgreSQL credentials if needed

## Running the Application

```bash
dotnet run
```

The application will:
1. Connect to the PostgreSQL database
2. Schedule a recurring job that runs every minute
3. Read data from the database
4. Wait 2 seconds
5. Print the data to the console

## How It Works

- Uses Hangfire with PostgreSQL storage for job scheduling
- The `DataService` class handles database operations
- Jobs are scheduled using Hangfire's `RecurringJob` feature
- The job runs automatically every minute without manual intervention