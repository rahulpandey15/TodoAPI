# Todo API with Clean Architecture (.NET)

A production-ready **Todo API** built with **.NET** following **Clean Architecture** principles.  
This project is designed as a learning resource and real-world reference for building scalable, maintainable, and testable APIs.

Alongside the source code, I’m also publishing a **https://www.youtube.com/playlist?list=PLWXMCIy8Ap7lnZ1h0Yaqnn2q4k8HExP5a** where I build this project step-by-step from scratch.

---

## 🚀 Project Goal

The objective of this repository is to demonstrate how to build a modern backend API using:

- Clean Architecture
- Domain-Driven Design principles
- SOLID principles
- Dependency Injection
- Entity Framework Core
- Repository & Unit of Work patterns
- Authentication & Authorization (planned)
- CQRS (planned)
- Testing (planned)
- Docker & Deployment (planned)

---

## 🏗️ Architecture Overview

This project follows **Clean Architecture**, separating responsibilities into independent layers:

```text
src/
├── TodoApi.API              --> Presentation Layer
├── TodoApi.Application      --> Use Cases / Business Logic
├── TodoApi.Domain           --> Core Entities / Rules
├── TodoApi.Infrastructure   --> Database / External Services
