# Todo API with Clean Architecture (.NET)

A production-ready **Todo API** built with **.NET** following **Clean Architecture** principles.  
This project is designed as a learning resource and real-world reference for building scalable, maintainable, and testable APIs.

Alongside the source code, I’m also posting video on my Youtube Channel, the playlist link is **https://www.youtube.com/playlist?list=PLWXMCIy8Ap7lnZ1h0Yaqnn2q4k8HExP5a** where I build this project step-by-step from scratch.

---

## Project Goal

The objective of this repository is to demonstrate how to build a modern backend API using:

- Clean Architecture
- Domain-Driven Design principles
- SOLID principles
- Dependency Injection
- Entity Framework Core
- Repository & Patterns
- Authentication & Authorization (planned)
- Docker & Deployment (planned)

---

## Architecture Overview

This project follows **Clean Architecture**, separating responsibilities into independent layers:

```text
src/
├── TodoApi.API              --> Presentation Layer
├── TodoApi.Application      --> Use Cases / Business Logic
├── TodoApi.Domain           --> Core Entities / Rules
├── TodoApi.Infrastructure   --> Database / External Services
```

---
## How  to Run This Application Locally
1. Download the source code and navigate to `appsetting.Development.json` file. 
2. Replace `ConnectionString` with your connection string
3. Install EFCore Cli Tool https://learn.microsoft.com/en-us/ef/core/cli/dotnet
4. Run the migration commands to create database locally
5. The commands are as follows
   1. `dotnet ef database update --startup-project .\Todo.API\ --project .\Todo.Infrastructure\`
   2. This should create a database in your local machine 
6. If you notice any issue in this process, reachout to my discord channel for support. https://discord.gg/Szs9ypCdRj
