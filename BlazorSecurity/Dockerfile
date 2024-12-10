# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

EXPOSE 8000
EXPOSE 8001

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the app
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Start the application
ENTRYPOINT ["dotnet", "BlazorSecurity.dll"]
