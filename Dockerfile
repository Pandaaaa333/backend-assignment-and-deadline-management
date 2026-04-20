# Stage 1: Build Image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for better caching
COPY ["backend_assignment_and_deadline_management_project.sln", "./"]
COPY ["backend_assignment_and_management_project.API/backend_assignment_and_management_project.API.csproj", "backend_assignment_and_management_project.API/"]
COPY ["backend_assignment_and_management_project.Application/backend_assignment_and_management_project.Application.csproj", "backend_assignment_and_management_project.Application/"]
COPY ["backend_assignment_and_management_project.Domain/backend_assignment_and_management_project.Domain.csproj", "backend_assignment_and_management_project.Domain/"]
COPY ["backend_assignment_and_management_project.Infrastructure/backend_assignment_and_management_project.Infrastructure.csproj", "backend_assignment_and_management_project.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "backend_assignment_and_deadline_management_project.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the API project
WORKDIR "/src/backend_assignment_and_management_project.API"
RUN dotnet build "backend_assignment_and_management_project.API.csproj" -c Release -o /app/build
RUN dotnet publish "backend_assignment_and_management_project.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports (adjust if your project uses different ones)
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "backend_assignment_and_management_project.API.dll"]
