# AbjjadAssignment

This guide will help you locally set up and run the **AbjjadAssignment** project using Docker.

## Prerequisites

Ensure you have the following tools installed on your machine:
- [Docker](https://docs.docker.com/get-docker/) (version 20.10.x or above)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 1.29.x or above)

## Project Structure
```
├── Dockerfile
├── docker-compose.yml
├── AbjjadAssignment.csproj
├── /ImageStorage (Directory for storing image files)
├── /src (Your source code files)
```

## Getting Started

1. **Clone the repository**
```bash
git clone https://github.com/Ka8eeM/AbjjadAssignment.git
cd AbjjadAssignment
```

2. **Build and Run the Containers**  
From the root directory of your project (where `docker-compose.yml` is located), run:
```bash
docker-compose up --build
```
This will:
- Build the .NET application using the `Dockerfile`.
- Set up the `abjjadassignment` service.
- Map port `5000` from the container to port `5000` on your host machine.
- Mount the `abjjadassignment-storage` volume to persist data in `/app/ImageStorage`.

3. **Access the Application**  
Once the containers are up, the application will be running at:
```
http://localhost:5000/swagger/index.html
```

4. **Health Check**  
The service includes a health check endpoint at:
```
http://localhost:5000/health
```
You can confirm if the service is healthy by running:
```bash
curl http://localhost:5000/health
```
If the service is healthy, you will receive an HTTP 200 response.

5. **Stopping the Containers**
To stop the running containers, press `Ctrl+C` in the terminal where you ran `docker-compose up`.  
To stop and remove the containers, networks, and volumes, run:
```bash
docker-compose down
```

6. **Rebuilding the Containers** (If you make changes to your application):
```bash
docker-compose up --build
```

## Troubleshooting
- If you encounter permission issues with the `ImageStorage` directory, try adjusting the permissions on your local machine.
- Ensure Docker Desktop or Docker Engine is running before you attempt to build or run the containers.
- If health checks fail, ensure your application is properly listening on `http://+:5000` within the container.

## Additional Notes
- The application is built on **.NET 8.0**.
- `libgdiplus` is installed for compatibility with **ImageSharp** for image processing.
- The `ASPNETCORE_ENVIRONMENT` is set to `Development` by default. Adjust this as needed.


