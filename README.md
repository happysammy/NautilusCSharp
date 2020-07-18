![Nautech Systems](https://github.com/nautechsystems/nautilus_trader/blob/master/docs/artwork/ns-logo.png?raw=true "logo")

----------

# Nautilus

![Build](https://github.com/nautechsystems/Nautilus/workflows/build/badge.svg)
![Code Coverage](https://img.shields.io/codecov/c/github/nautechsystems/Nautilus)
## Introduction

Nautilus is an enterprise grade distributed algorithmic trading platform allowing flexible deployment 
topologies including embedded/local on a single machine - or distributed across a Cloud/VPC.
Architectural methodologies include domain driven design, event-sourcing and messaging.

Nautilus is written entirely in C# for .NET Core and has been open-sourced from working production code.
Nautilus forms part of larger infrastructure designed and built to support the trading operations of 
professional quantitative traders and/or small hedge funds.

The platform exists to support the NautilusTrader algorithmic trading framework with distributed services 
to facilitate live trading. NautilusTrader heavily utilizes Cython to provide type safety and performance 
through C extension modules.

> https://github.com/nautechsystems/nautilus_trader

This means the Python ecosystem can be fully leveraged to research, backtest and trade strategies developed 
through machine learning techniques, with data ingest, order management and risk management
being handled by the Nautilus platform services. 

Each Nautilus service uses a common intra-service messaging library built on top of the Task Parallel Library 
(TPL) Dataflow, which allows the service sub-components to connect to central message buses to fully utilize 
every available thread.

> https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

An efficient inter-service messaging system implemented using ZeroMQ for transport, MessagePack serialization
and Curve25519 encryption allows extremely fast communication, with the API allowing PUB/SUB and 
fully async REQ/REP patterns. There are plans to develop an optional Google Protobuf adapter.

The Order Management System (OMS) includes an `ExecutionEngine` with underlying `ExecutionDatabase`
built on top of Redis, which supports the ability to manage global risk across many trader machines.

The repository is grouped into the following solution folders;
- `Framework` provides the domain model and common components for implementing the services.
- `Services` provides generic data and execution services.
- `Infrastructure` provides technology specific implementations. At present utilizing Redis.
- `Adapters` provides broker specific implementations. At present supporting `FIX4.4` with FXCM.
- `Applications` provides `NautilusData` and `NautilusExecutor` ASP.NET Core applications.
- `TestSuite` provides the unit and integration tests for the codebase.

There is currently a large effort to develop improved documentation.

## Values

* Reliability
* Availability
* Testability
* Performance
* Modularity
* Maintainability
* Scalability

## Support
Please direct all questions, comments or bug reports to info@nautechsystems.io

Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.

> https://nautechsystems.io

![DotNet](https://d585tldpucybw.cloudfront.net/sfimages/default-source/default-album/net-core-3_480.png?sfvrsn=42bb708c_0?raw=true "dotnet")
