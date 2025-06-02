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

The application is configured for automatic deployment to a VPS using GitHub Actions.

### Server Configuration

PostgreSQL is already deployed and configured on the VPS to accept connections from Docker containers:

- PostgreSQL runs directly on the VPS (not in a container)
- The web application container connects to PostgreSQL using `host.docker.internal` as the hostname
- The connection string in the container is configured as: `Host=host.docker.internal;Database=psgendb;Username=psgenuser;Password=your_secure_password`

### Container Deployment

The application is deployed as a Docker container to the same VPS where PostgreSQL is running. GitHub Actions automates this deployment process.

### Required GitHub Secrets

Set the following secrets in your GitHub repository:

- `VPS_HOST`: IP address of your VPS
- `VPS_USERNAME`: SSH username
- `VPS_SSH_KEY`: SSH private key for authentication (the entire key, including BEGIN and END lines)
- `DB_CONNECTION_STRING`: PostgreSQL connection string (format: `Host=host.docker.internal;Database=psgendb;Username=psgenuser;Password=your_secure_password`)

#### Setting up SSH Key for GitHub Actions

1. Generate a dedicated SSH key for GitHub Actions:
   ```
   ssh-keygen -t ed25519 -C "github-actions-deploy@psgen-api" -f ~/.ssh/github-actions/id_ed25519
   ```

2. Add the public key to your server's authorized_keys file:
   ```
   # On your VPS
   echo "your-public-key-here" >> ~/.ssh/authorized_keys
   ```

3. Add the private key to GitHub secrets as `VPS_SSH_KEY`

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

3. On the server, create a docker-compose.yml file with:
   ```yaml
   version: '3.8'
   
   services:
     psgen-api:
       image: your-registry/psgen-api:latest
       ports:
         - "5000:80"
       environment:
         - ASPNETCORE_ENVIRONMENT=Production
         - ConnectionStrings__DefaultConnection=Host=host.docker.internal;Database=psgendb;Username=psgenuser;Password=your_secure_password
       extra_hosts:
         - "host.docker.internal:host-gateway"
       restart: unless-stopped
   ```

4. Pull and run:
   ```
   docker-compose pull
   docker-compose up -d
   ```

## API Documentation

The API documentation is available through Swagger UI at `/swagger` when the application is running.

## Database Migrations

When deploying for the first time or after schema changes, run migrations:

```
# For local development:
cd src
dotnet ef database update

# On the server (if needed):
# Option 1: Run migrations through the API endpoint
curl -X POST https://your-domain.com/api/migration

# Option 2: Execute migrations inside the container
docker exec -it psgen-api_psgen-api_1 dotnet ef database update
```

## Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to Development, Staging, or Production
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
