![Nautech Systems](https://github.com/nautechsystems/nautilus_trader/blob/master/docs/artwork/nautech-systems-logo.png?raw=true "logo")

----------

# NautilusCSharp

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/fb2f64bf5cf8468983dbbd841d29c4e3)](https://app.codacy.com/gh/nautechsystems/NautilusCSharp?utm_source=github.com&utm_medium=referral&utm_content=nautechsystems/NautilusCSharp&utm_campaign=Badge_Grade_Dashboard)
![Build](https://img.shields.io/github/workflow/status/nautechsystems/NautilusCSharp/build)

**BETA**

- **This repo is currently unmaintained and will potentially become a C# reference architecture**
- **The integration with NautilusTrader has breaking changes**
- **The API is still in a state of flux with potential further breaking changes**

## Introduction

NautilusCSharp is a back-end infrastructure suite supporting algorithmic trading operations. 
Flexible deployment topologies facilitate running services both embedded/local on a single machine, 
or distributed across a Cloud/VPC. Architectural methodologies include domain driven design, 
event-sourcing and messaging.

Nautilus is written entirely in C# for .NET Core and has been open-sourced from working production code.
Nautilus forms part of larger infrastructure designed and built to support the trading operations of 
professional quantitative traders and/or small hedge funds.

The platform exists to support the NautilusTrader algorithmic trading framework with distributed services 
to facilitate live trading. NautilusTrader heavily utilizes Cython to provide type safety and performance 
through C extension modules.

> https://github.com/nautechsystems/nautilus_trader

This means the Python ecosystem can be fully leveraged to research, backtest and trade strategies developed 
through AI/ML techniques, with data ingest, order management and risk management
being handled by the Nautilus platform services.

Each Nautilus service uses a common intra-service messaging library built on top of the Task Parallel Library 
(TPL) Dataflow, which allows the service sub-components to connect to central message buses to fully utilize 
every available thread.

> https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

An efficient inter-service messaging system implemented using MessagePack serialization,
LZ4 compression, Curve25519 encryption and ZeroMQ transport - allows extremely fast communication, with the 
API allowing PUB/SUB and fully async REQ/REP patterns.

The Order Management System (OMS) includes an `ExecutionEngine` with underlying `ExecutionDatabase`
built on top of Redis, which supports the ability to manage global risk across many trader machines.

The repository is grouped into the following solution folders;
  - `Framework` provides the domain model and common components for implementing the services.
  - `Services` provides generic data and execution services.
  - `Infrastructure` provides technology specific implementations. At present utilizing Redis.
  - `Adapters` provides broker specific implementations. At present supporting `FIX4.4` with FXCM.
  - `TestSuite` provides the unit and integration tests for the codebase.

There is currently a large effort to develop improved documentation.

## Values

  - Reliability
  - Availability
  - Testability
  - Performance
  - Modularity
  - Maintainability
  - Scalability

## Support
Please direct all questions, comments or bug reports to info@nautechsystems.io

Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.

> https://nautechsystems.io

![DotNet](https://d585tldpucybw.cloudfront.net/sfimages/default-source/default-album/net-core-3_480.png?sfvrsn=42bb708c_0?raw=true "dotnet")
