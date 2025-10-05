# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY Chair.Api/Chair.Api.csproj Chair.Api/
COPY Chair.Domain/Chair.Domain.csproj Chair.Domain/
COPY Chair.Infrastructure/Chair.Infrastructure.csproj Chair.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all code
COPY . ./

# Publish the API project
RUN dotnet publish Chair.Api/Chair.Api.csproj -c Release -o out

# STEP 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/out ./

# Set the port that Render will provide
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

# Run the app
ENTRYPOINT ["dotnet", "Chair.Api.dll"]
