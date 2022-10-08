# Motivation
Waking up one day. I was totally in a panic because I had to release a new version of my restaurant manager app to solve a problem of a restaurant near my home.

Unfortunately, this app doesn't exist yet. So I create this solution to sleep better in the future.

# Goal
Goal of this project is to try out new stuff and to playaround with it. It is no production ready solution.

# Architecture
This project uses a microservice architecture. My current understanding of github is that you have to put each microservice into one git repository. All of the repository start with `restaurant`, followed by the domain and the type of application. I like things to be sorted in alphabetical order.

# Code Design
This project uses my interpreation of [domain driven design](https://martinfowler.com/tags/domain%20driven%20design.html). I was forced to follow anti pattern like [AnemicDomainModel](https://martinfowler.com/bliki/AnemicDomainModel.html) in the past. I do my best to convince people that the patterns are not only theory. Building production like demonstration in C# is my way to reach that.

The solution uses [Onion Architecture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)/[Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) to support DDD and Dependency Injection. This enables use to devolope [modern and testable](https://docs.microsoft.com/de-de/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures) applications.

I know that the architecture do not have to be so complex for microservices. You have a low complexity inside your service. But i generally like to separate things but i do not use different projects to enforce the architecture. I currently have a circular dependency between domain and persistence because the source event [OrderCreated](Restaurant.SvcOrder/Domain/Orders/SourceEvents/OrderCreated.cs) uses the [OrderCreatedMapper](Restaurant.SvcOrder/Repositories/Orders/SourceEvents/OrderCreatedMapper.cs) as static class. I may remove this in the future, but i am still bound to IMessage in the interface [ISourceEvent](Restaurant.SvcOrder/Domain/SourceEvents/ISourceEvent.cs).

# Automation
Every time you push something a [pipeline](.github/workflows/pipeline.yml) will run. As developers, we are working to automate processes and thus eliminate manual work. There's no excuse why we have to do things manually.

![example event parameter](https://github.com/kinneko-de/restaurant-order-svc/actions/workflows/pipeline.yml/badge.svg?event=push)

# Logging
Is out of scope for now.

# Database
See [Database-Readme](database/README.md) for further information.

# Tests
Write tests when ever they are needed and are useful.

Never try to spare time here. You have to invest more time later.

Never write useless tests to get a high code coverage.

[![codecov](https://codecov.io/gh/KinNeko-De/restaurant-order-svc/branch/master/graph/badge.svg?token=F2ADS06FGH)](https://codecov.io/gh/KinNeko-De/restaurant-order-svc)

Tests need to be maintained as normla code. Keep that in mind while you write tests.

# Further documentation
See [Documentation](docs/README.md).
