#!/bin/bash
set -e

echo "Waiting for SQL Server..."
for i in {1..30}; do
  if sqlcmd -S sqlserver -U sa -P 'YourStrong@Passw0rd123' -C -Q "SELECT 1" > /dev/null 2>&1; then
    echo "SQL Server is ready!"
    break
  fi
  sleep 2
done

echo "Running migrations..."
cd /src
dotnet ef database update

echo "Starting application..."
exec dotnet run --no-build
