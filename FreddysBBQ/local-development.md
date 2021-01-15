# Local Development

## Dependencies

- Docker

## Load Dependent Services

### MySQL DB

Create MySQL Docker Container

`docker run -d --name steeltoe-mysql -p 3306:3306 -e MYSQL_DATABASE=steeltoe -e MYSQL_ROOT_PASSWORD=password mysql`

### Existing UAA

`docker run -d -ti -p 8080:8080 --name steeltoe-uaa steeltoeoss/workshop-uaa-server`

### Local UAA

- TODO: Add instructions to pull down uaa

Confirm Local UAA Setup

Running

```
curl 'http://localhost/oauth/token' -i -X POST \
    -H 'Content-Type: application/x-www-form-urlencoded' \
    -H 'Accept: application/json' \
    -d 'client_id=login&client_secret=loginsecret&scope=scim.write&grant_type=client_credentials&token_format=opaque'
```

Should Return

```json
{
  "access_token": "ecca03de7cec4138b813b990024291f6",
  "token_type": "bearer",
  "expires_in": 43199,
  "scope": "clients.read clients.secret clients.write scim.read scim.write",
  "jti": "ecca03de7cec4138b813b990024291f6"
}
```

<i>TODO: Password grant not yet working</i>

```
`curl -v -d"username=user1&password=password&client_id=orderserviceapi&grant_type=password" -u "orderserviceapi:o463qXwa" http://localhost:8080/uaa/oauth/token`
```

## Eureka Server

`docker run -d --name steeltoe-eureka -p 8761:8761 steeltoeoss/eureka-server`

TODO:

- Password grant access
- Autogenerate DB in new OrderService Project
  - <i>For now add `dotnet ef database update instructions`</i>
