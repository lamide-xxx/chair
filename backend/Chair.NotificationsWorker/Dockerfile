# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY Chair.Domain/Chair.Domain.csproj Chair.Domain/
COPY Chair.NotificationsWorker/Chair.NotificationsWorker.csproj Chair.NotificationsWorker/

RUN dotnet restore

COPY . ./
RUN dotnet publish Chair.NotificationsWorker/Chair.NotificationsWorker.csproj -c Release -o out

# STEP 2: Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Chair.NotificationsWorker.dll"]
