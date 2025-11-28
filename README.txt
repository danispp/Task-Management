
A full-stack web application for managing projects and tasks, built with .NET 8 and Angular 17.

## Features

- **User Authentication**: Secure registration and login with JWT tokens
- **Project Management**: Create, view, and delete projects
- **Task Management**: Add tasks to projects, update status (Todo/In Progress/Done), and track progress
- **Responsive Design**: Clean, modern UI that works on desktop and mobile

## Tech Stack

### Backend
- **.NET 8 Web API** - RESTful API with C#
- **Entity Framework Core** - Database ORM with Code First migrations
- **SQL Server** - Relational database
- **JWT Authentication** - Secure token-based auth
- **BCrypt** - Password hashing

### Frontend
- **Angular 17** - TypeScript-based SPA framework
- **RxJS** - Reactive programming for HTTP requests
- **Angular Router** - Client-side routing with guards
- **Reactive Forms** - Form validation and handling

### Infrastructure
- **Docker & Docker Compose** - Containerized deployment
- **Nginx** - Web server for Angular app

## Prerequisites

Before you begin, ensure you have the following installed:

- **Docker** (version 20.10 or higher)
- **Docker Compose** (version 1.29 or higher)

That's it! Docker handles everything else (no need for .NET SDK, Node.js, or SQL Server locally).

### Installing Docker

**Ubuntu/Debian:**
```bash
sudo apt update
sudo apt install docker.io docker-compose
sudo usermod -aG docker $USER
# Log out and back in for group changes to take effect
```

**Windows/Mac:**
Download Docker Desktop from https://www.docker.com/products/docker-desktop

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd fullstack
```

### 2. Start the Application
```bash
docker-compose up -d
```

This will:
- Pull required Docker images (first time only)
- Build the backend and frontend containers
- Start SQL Server
- Run database migrations automatically
- Start all services in the background

**First startup takes 2-3 minutes.** Subsequent starts are much faster (~10 seconds).

### 3. Access the Application

Open your browser and go to:

**http://localhost:4200**

## Using the Application

### First Time Setup

1. **Register an Account**
   - Click "Register here" on the login page
   - Enter your email, password, and full name
   - Click "Register"
   - You'll be automatically logged in

2. **Create Your First Project**
   - Click the "+ Create New Project" card
   - Enter a project name and description
   - Click "Create"

3. **Add Tasks to Your Project**
   - Click on a project card to open it
   - Click "+ Add Task"
   - Enter task title and optional description
   - Click "Create"

4. **Manage Tasks**
   - Click status buttons (Todo/In Progress/Done) to update task status
   - Click the × button to delete a task
   - Click "← Back" to return to projects list

### API Documentation

Once the application is running, you can explore the API endpoints at:

**http://localhost:5000/swagger**

This provides interactive API documentation where you can test endpoints directly.

## Management Commands

### Viewing Logs
```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View logs for specific service
docker-compose logs backend
docker-compose logs frontend
docker-compose logs sqlserver
```

### Stopping the Application
```bash
# Stop containers (data persists)
docker-compose down

# Stop and remove all data (fresh start)
docker-compose down -v
```

### Restarting Services
```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart backend
```

### Rebuilding After Code Changes
```bash
# Rebuild and restart
docker-compose up -d --build
```

## Database Management

### Accessing the Database
```bash
# Connect to SQL Server container
docker exec -it taskmanagement-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd123' -C
```

Then run SQL commands:
```sql
USE TaskManagementDB;
GO

-- View all users
SELECT * FROM Users;
GO

-- View all projects
SELECT * FROM Projects;
GO

-- View all tasks
SELECT * FROM Tasks;
GO
```

### Using a GUI Tool

You can connect with tools like:
- **DBeaver** (free, cross-platform)
- **Azure Data Studio** (free, from Microsoft)
- **SQL Server Management Studio** (Windows only)

**Connection Settings:**
- Host: `localhost`
- Port: `1433`
- Database: `TaskManagementDB`
- Username: `sa`
- Password: `YourStrong@Passw0rd123`

## Architecture

### Backend API Endpoints

**Authentication:**
- `POST /api/auth/register` - Create new user account
- `POST /api/auth/login` - Login and receive JWT token

**Projects:**
- `GET /api/projects` - Get all projects for logged-in user
- `GET /api/projects/{id}` - Get project details with tasks
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

**Tasks:**
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `PUT /api/tasks/{id}/status` - Update task status
- `DELETE /api/tasks/{id}` - Delete task

### Database Schema

**Users Table:**
- Id (Primary Key)
- Email (Unique)
- PasswordHash
- FullName
- CreatedAt

**Projects Table:**
- Id (Primary Key)
- Name
- Description
- UserId (Foreign Key → Users)
- CreatedAt

**Tasks Table:**
- Id (Primary Key)
- Title
- Description
- Status (0=Todo, 1=InProgress, 2=Done)
- Priority (0=Low, 1=Medium, 2=High)
- ProjectId (Foreign Key → Projects)
- AssignedToUserId (Foreign Key → Users, nullable)
- CreatedAt
- DueDate (nullable)

### Authentication Flow

1. User registers/logs in
2. Backend generates JWT token
3. Frontend stores token in localStorage
4. All subsequent requests include token in Authorization header
5. Backend validates token and extracts user ID
6. API returns only data belonging to authenticated user

## Troubleshooting

### Port Conflicts

If you get "port already allocated" errors:
```bash
# Check what's using the port
sudo lsof -i :5000   # Backend
sudo lsof -i :4200   # Frontend
sudo lsof -i :1433   # Database

# Stop the conflicting process
sudo kill -9 <PID>
```

### Containers Won't Start
```bash
# Check container status
docker-compose ps

# View detailed logs
docker-compose logs

# Force rebuild
docker-compose down -v
docker-compose up --build
```

### Database Connection Issues
```bash
# Ensure SQL Server is running
docker-compose ps sqlserver

# Check SQL Server logs
docker-compose logs sqlserver

# Restart SQL Server
docker-compose restart sqlserver
```

### Frontend Shows Blank Page
```bash
# Check frontend logs
docker-compose logs frontend

# Verify build completed successfully
docker-compose up --build frontend
```

## Development

### Making Code Changes

**Backend Changes:**
```bash
# Edit files in Backend/
# Then rebuild and restart
docker-compose up -d --build backend
```

**Frontend Changes:**
```bash
# Edit files in Frontend/src/
# Then rebuild and restart
docker-compose up -d --build frontend
```

### Running Without Docker (Development)

**Backend:**
```bash
cd Backend
dotnet restore
dotnet ef database update
dotnet run
```

**Frontend:**
```bash
cd Frontend
npm install
ng serve
```

## Security Considerations

**For Production Deployment:**

1. **Change Default Passwords**
   - Update SQL Server password in docker-compose.yml
   - Update connection strings accordingly

2. **Use Environment Variables**
   - Store secrets in .env file
   - Never commit passwords to Git

3. **Enable HTTPS**
   - Configure SSL certificates
   - Update nginx configuration

4. **Implement Rate Limiting**
   - Prevent brute force attacks
   - Add rate limiting middleware

5. **Regular Updates**
   - Keep Docker images updated
   - Update NuGet and npm packages

## Performance Optimization

**Production Recommendations:**

1. Use production Docker images (smaller size)
2. Enable response compression
3. Implement caching strategies
4. Use CDN for static assets
5. Configure database connection pooling
6. Add indexes to frequently queried columns

## License

This project is open source and available under the MIT License.

## Support

If you encounter any issues:

1. Check the Troubleshooting section above
2. Review Docker logs: `docker-compose logs`
3. Ensure all prerequisites are installed correctly
4. Try a fresh start: `docker-compose down -v && docker-compose up --build`

## Acknowledgments

Built with:
- [.NET 8](https://dotnet.microsoft.com/)
- [Angular 17](https://angular.io/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Docker](https://www.docker.com/)

---

**Note:** This is a demonstr
