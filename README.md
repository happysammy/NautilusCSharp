![Nautech Systems](https://github.com/nautechsystems/nautilus_trader/blob/master/docs/artwork/nautechsystems_logo_small.png?raw=true "logo")

----------

# Nautilus

#### Build Status

![Build Status](https://codebuild.ap-southeast-2.amazonaws.com/badges?uuid=eyJlbmNyeXB0ZWREYXRhIjoiQ0RNcmVkNnl6M2p2RURYb1RmUzlLWFlLTForVVJDb2hnTXluWVRxdENMSGlDVXZYTmtHZDlnOHhENG9tZEdibXRXeFZwRzRVNUdoMWF6U2xQN05EbDhBPSIsIml2UGFyYW1ldGVyU3BlYyI6InQ1Tkhxa0RFYldKNDAwcVIiLCJtYXRlcmlhbFNldFNlcmlhbCI6MX0%3D&branch=master)

## Introduction

Nautilus is an algorithmic trading platform allowing flexible deployment 
topologies including embedded/local on a single machine - or distributed across a VPC.
Architecutral methodologies include domain driven design, event-sourcing, immutable value types 
and message passing.

Nautilus has been open-sourced from working production code and exists to support 
the NautilusTrader Python algorithmic trading framework https://github.com/nautechsystems/nautilus_trader 
by providing `Data` and `Execution` services. 

A messaging system API implemented using ZeroMQ transport, MessagePack serialization
and Curve25519 encryption allows efficient communication between the services and trader
machines through PUB/SUB and fully async REQ/REP patterns.

An `ExecutionEngine` with underlying `ExecutionDatabase` including a Redis implementation
supports the ability to manage global risk across many trader machines.

The repository is grouped into the following sections;
- `Framework` provides the core and common components which allow the services to be implemented.
- `Services` provides the main data and execution services.
- `Infrastructure` provides specific implementations. At present utilizing Redis.
- `Adapters` provides broker specific implementations. At present supporting FIX4.4 with FXCM.
- `TestSuite` provides unit and integration tests for the codebase.

There is currently a large effort to provide improved documentation.

## Values
* Reliability
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
