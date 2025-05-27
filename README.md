# PsGen API

This is a .NET 9 Web API application for password generation and management.

## Development Setup

### Prerequisites

- .NET 9 SDK
- Docker and Docker Compose
- Git

### Local Development

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/PsGen-Api.git
   cd PsGen-Api/psgen-api
   ```

2. Run with Docker Compose:
   ```
   docker-compose up
   ```

3. Or run locally:
   ```
   cd src
   dotnet run
   ```

The API will be available at:
- http://localhost:5000
- https://localhost:5001

Swagger UI: https://localhost:5001/swagger

## Production Deployment

The application is configured for automatic deployment to a Hetzner Ubuntu VM using GitHub Actions.

### Required GitHub Secrets

Set the following secrets in your GitHub repository:

- `HETZNER_HOST`: IP address of your Hetzner server
- `HETZNER_USERNAME`: SSH username
- `HETZNER_SSH_KEY`: SSH private key for authentication
- `DB_CONNECTION_STRING`: PostgreSQL connection string
- `DB_USER`: PostgreSQL username
- `DB_PASSWORD`: PostgreSQL password
- `DB_NAME`: PostgreSQL database name
- `JWT_KEY`: Secret key for JWT token signing (at least 32 characters)

### Manual Deployment

1. Build the Docker image:
   ```
   docker build -t psgen-api .
   ```

2. Push to your container registry:
   ```
   docker tag psgen-api your-registry/psgen-api:latest
   docker push your-registry/psgen-api:latest
   ```

3. On the server, create a docker-compose.yml file (see the GitHub Action workflow)

4. Pull and run:
   ```
   docker-compose pull
   docker-compose up -d
   ```

## API Documentation

The API documentation is available through Swagger UI at `/swagger` when the application is running.

## Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to Development, Staging, or Production
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `Jwt__Key`: Secret key for JWT token signing
- `Jwt__Issuer`: JWT issuer (default: psgen-api)
- `Jwt__Audience`: JWT audience (default: psgen-clients)
- `Jwt__ExpiryInDays`: JWT token expiry in days (default: 90)
