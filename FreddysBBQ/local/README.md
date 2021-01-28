# Local Development

## Dependencies

- Docker
- Java 11 (for local uaa)

## Load Dependent Services

### MySQL DB

Create MySQL Docker Container

`docker run -d --name steeltoe-mysql -p 3306:3306 -e MYSQL_DATABASE=steeltoe -e MYSQL_ROOT_PASSWORD=password mysql`

### Local UAA

#### Retrieve local uaa repository

[CloudFoundry/UAA](https://github.com/cloudfoundry/uaa)

#### Run UAA

Place [uaa.yml](./uaa.yml) from this directory in local uaa repository directory at below path:

`<uaa root directory>\uaa\src\main\resources`

Run uaa locally with `gradlew run`

#### Confirm Local UAA Setup

Running the following command

```
curl -X GET -H "Accept: application/json" -H "Content-Type: application/json" http://localhost:8080/uaa/info
```

Should Return

```json
{
  "app": {
    "version": "<your version>"
  },
  "links": {
    "uaa": "http://localhost:8080/uaa",
    "login": "http://localhost:8080/uaa"
  },
  "zone_name": "uaa",
  "prompts": {
    "username": ["text", "Email"],
    "password": ["password", "Password"]
  },
  "timestamp": "2021-01-27T18:07:43-0600"
}
```

## Eureka Server

`docker run -d --name steeltoe-eureka -p 8761:8761 steeltoeoss/eureka-server`

## Local Application

### Project Startup

Set multiple project startup for AdminPortal and OrderService

1. Open solution in Visual Studio
1. Right-click solution in Solution Explorer
1. Select "Set Startup Projects"
1. Select "Multiple Project Startup" option
1. Set AdminPortal and OrderService action to "Start"
1. Click "Apply", then "OK"
1. Run project

<i>\*Note: Steeltoe versions earlier than 3.1.0. Copy and paste the contents of the appsettings json in this local directory to the respective projects to handle url overrides.</i>

<i>\*Note: If you do not have the Java based menu service running, you can still run the application locally by replacing MenuService with the MockMenuService in the Startup.cs dependency injection for the OrderSercice and AdminPortal projects</i>

```csharp
// replace
services.AddTransient<IMenuService, MenuService>();

// with
services.AddTransient<IMenuService, MockMenuService>();
```
