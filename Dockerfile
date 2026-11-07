# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["JanSamadhan.csproj", "./"]
RUN dotnet restore "JanSamadhan.csproj"

# Copy remaining source code and publish
COPY . .
RUN dotnet publish "JanSamadhan.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

# Create persistent data and uploads directory structure
RUN mkdir -p data wwwroot/uploads/comments wwwroot/uploads/technician

ENTRYPOINT ["dotnet", "JanSamadhan.dll"]

