## E-commerce application using Microservices with .NET Core
<div align="center">
  <img src="https://badges.pufler.dev/visits/0xtuanit/ecommerce_microservices" alt="Years Badge">
  <img src="https://badges.pufler.dev/updated/0xtuanit/ecommerce_microservices" alt="Repos Badge">
  <img src="https://badges.pufler.dev/created/0xtuanit/ecommerce_microservices" alt="Gists Badge">
</div>

## Prepare environment:

* Install dotnet core version in file `global.json`
* IDE: Visual Studio 2022+, Rider, Visual Studio Code
* Docker Desktop

## Architecture overview:
- Introduction:
    - The project is Micro-services based architecture where each separate service can use different technologies.</br>
      E.g. Product uses MySQL, Customer uses PostgresQL, Inventory uses MongoDB and so on.
    - By the end of the day, these separate services can get connected and talked to each other though they use different technologies. So, together they will get a full flow of features completed for the whole system.
-  Screenshot:</br>
    <p align="center">
      <img width="700" height="400" alt="Architecture" src="https://github.com/user-attachments/assets/0e7ca967-ace4-41e7-8af3-a8665588c599" />
    </p>


## Warning:

Some docker images are not compatible with Apple Chip (M1, M2). You should replace them with appropriate images. Suggestion images below:
- sql server: mcr.microsoft.com/azure-sql-edge
- mysql: arm64v8/mysql:oracle
---
## How to run the project

Run command for build project
```Powershell
dotnet build
```
Go to folder contain file `docker-compose`

1. Using docker-compose
```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

## Application URLs - LOCAL Environment (Docker Container):
- Product API: http://localhost:6002/api/products
- Customer API: http://localhost:6003/api/customers
- Basket API: http://localhost:6004/api/baskets

## Docker Application URLs - LOCAL Environment (Docker Container):
- Portainer: http://localhost:9000 - username: admin ; pass: admin
- Kibana: http://localhost:5601 - username: elastic ; pass: admin
- RabbitMQ: http://localhost:15672 - username: thomas ; pass: Admin3000

2. Using Visual Studio 2022
- Open aspnetcore-microservices.sln - `aspnetcore-microservices.sln`
- Run Compound to start multi-projects
---
## Application URLs - DEVELOPMENT Environment:
- Product API: http://localhost:5002/api/products
- Customer API: http://localhost:5003/api/customers
- Basket API: http://localhost:5004/api/baskets
---
## Application URLs - PRODUCTION Environment:

---
## Packages References

## Install Environment

- https://dotnet.microsoft.com/download/dotnet/6.0
- https://visualstudio.microsoft.com/

## References URLS
- https://github.com/jasontaylordev/CleanArchitecture

## Docker Commands: (cd into folder contain file `docker-compose.yml`, `docker-compose.override.yml`)

- Up & running:
```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans --build
```
- Stop & Removing:
```Powershell
docker-compose down
```

## Useful commands:

- ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
- dotnet watch run --environment "Development"
- dotnet restore
- dotnet build
- Migration commands for Ordering API:
    - cd into Ordering folder
    - dotnet ef migrations add "Init_OrderDB" -p Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence/Migrations
    - dotnet ef migrations add "Order_Add_Status" -p Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence/Migrations
    - dotnet ef database update -p Ordering.Infrastructure --startup-project Ordering.API
- Revert everything before removing migrations when there's any error:
       // The number 0 is a special case that means before the first migration and causes all migrations to be reverted.
    - " dotnet ef database update 0 -p Ordering.Infrastructure --startup-project Ordering.API   
        dotnet ef migrations remove -p Ordering.Infrastructure --startup-project Ordering.API
        dotnet ef database drop -p Ordering.Infrastructure --startup-project Ordering.API " 
