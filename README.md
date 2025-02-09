# Hive Implementation

## Project Description

Hive Implementation is an implementation of the strategic board game Hive based on insect movements on a hexagonal board. It includes game mechanics, data models, factories for generating game elements, and repositories for database interactions.

Project consists of three main modules:

- Server Module – Implemented in ASP.NET Core, responsible for managing game sessions, handling client requests, and interacting with the database.
- Client Module – A Unity-based application that provides the user interface and communicates with the server.
- MongoDB database - storing all active game sessions

The client and server communicate using full-duplex WebSockets, implemented via SignalR. Additionally, the API provides HTTPS endpoints for access to application state for admin.

## Technologies Used

- Programming Language: C#

- Framework: ASP.Net (Server), Unity (Client)

- Dependency Management: NuGet

- Database: MongoDB

- Communication: Full duplex communication via WebSockets

## Projects in solution

- HiveGameAPI - project implementing majority of Asp.Net configuration and hubs to communicate clients
- HiveGame.BusinessLogic - main logic for server application. It includes factories, insect models (and their behaviour), game instance model, services and creating token implmementation
- HiveGame.Repositories - implementation of connection to MongoDB database
- HiveGame.Core - models used by either client and server. After bulding there is an dll file of this project created in Unity project
- HiveGame.Client - Unity-based client implementation
- HiveGame.Tests - unit tests and integration tests for server

## Config files

- HiveGameAPI/log4net.config - logging details for server
- HiveGameAPI/appsettings.config - WebSocket connection settings, authorization details, database connection details
- HiveGame.Client/settings.config - connection and logging settings for client