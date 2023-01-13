# MVP RoomPlanner
The `RoomPlanner` was part of the integration project in the **CAS Agile Software Engineering** at BFH (Berner Fachhochschule).  

![RoomPlanner Mockups](img/RoomPlanner.png)

## License information
Source code licensed unser **GPL-3.0**.
- html mail templates by André Lergier [lergier.ch](https://lergier.ch)
- timetable view by Grible [timetable.js repo](https://github.com/Grible/timetable.js)

## Development
### Prerequisite for development
- Visual Studio 2022 (recommended) or Rider
- .NET 6 SDK
- Docker in WSL2
- docker-compose

### Create database for development
1. Navigate into docker-compose directory for local-dev  
> cd compose\Development

2. Start docker-services
> docker-compose up -d

### Migrations
1. `cd src`
2. `dotnet ef --startup-project .\RoomPlanner.App\ --project .\RoomPlanner.Infrastructure\ migrations add AddInvitationId` (change name of migration)

## Open issues
- messy frontend code (js)
- limited responsiveness for mobile devices
- rooms can only be added directly on the database in this MVP

## Design
**State Machine Diagram**
![State diagram](img/StateDiagram.png)

Architecture            |  Context
:-------------------------:|:-------------------------:
![Layered architecture](img/LayeredArchitecture.png)  |  ![Context diagram](img/ContextualDiagram.png)

