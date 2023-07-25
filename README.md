# Messaging09

Messaging09 is an opinionated library which enables messaging in dotnet applications.
The goal of the project is to make it easy to connect and use a message broker from your app,
and also reducing the boilerplate needed.

For now, we only support the amqp(v1.0) protocol on the activemq artemis broker, but we'd like to support more in the
future. If this interests you, please submit an issue, or even a pull request.

## installing Messaging09.Amqp

```powershell
Install-Package Messaging09.Amqp
```

```bash
dotnet add package Messaging09.Amqp
```

### installing for dotnet applications with dotnet DI

This references the package above, so you only need to install this one.

```powershell
Install-Package Messaging09.Amqp.Extensions.DependencyInjection
```

```bash
dotnet add package Messaging09.Amqp.Extensions.DependencyInjection
```
