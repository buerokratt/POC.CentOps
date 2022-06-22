# Introduction

This document outlines the technical design for CentOps within a scope to cover functionalities for "Milestone 2 - MVP End-to-End Flow".

# Scope

The scope of current developments is to have fully functional proof of concept deployed at least on Azure.

Full scope of CentOps (planned) features and business architecture can be found on [centops-business-architecture.doc.md](https://github.com/buerokratt/CentOps/blob/main/docs/centops-business-architecture.doc.md).

# Technical stack

| Types of services               | Technical stack                                                                                | Comment                                                                                      |
| ------------------------------- | ---------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| All application functionalities | Custom .Net applications                                                                       | Possible to replace with BYK-stack or other applicable components                            |
| Database for testing purposes   | [InMemory](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli) | Will at some point be replaced with some persistent database engine                          |
| Persistent storage              | [Cosmos DB](https://en.wikipedia.org/wiki/Cosmos_DB)                                           | Not sure that NoSQL database is the best fit for persistent storage but we'll use it for now |
| Logs                            | [Grafana Loki](https://grafana.com/oss/loki/)                                                  | Only fetching logs from Participants is currently in scope                                   |

# Authentication

## API keys

In scope of phase 1, Participants use API keys to identify them. This is also the basis for CentOps to validate if API keys provided by requesters have sufficient permissions to perform these specific actions.

**In the long run,** API keys will only be used as short-living, once-only usable keys for initial acceptance of new Participants. After that, more sophisticated authentication methods will be taken into use.

## CentOps permissions validation

**Within phase 1,** we won't even use API keys to validate permissions for requests. All requests come via Admin API and as stated in section **"Authenticate with CentOps API",** "*Applications also assume that all incoming requests come from appropriate participants with appropriate rights without any restrictions based on network, certificates etc.*"

This is because we use Ruuter as a reverse proxy in front of all of our back-end components which, among other things, allows us to validate user permissions the same way [(see first lines of this Ruuter DSL-to-be sample)](https://github.com/buerokratt/Ruuter/blob/main/samples/backlog/templated-services.md). This is a common practice for many projects using Ruuter, but as of writing this document, Ruuter 2.0 is in active development and we are not interested in using its previous (deprecated) version any more.

This means that regardless of if CentOps or any of its services are based on currently developed .Net or any other solutions, request authentication, permissions validation etc has been done before the request makes it to CentOps (or any other component / service), leaving application (like CentOps) just execute the requests without any additional validation.

# Features

## Authenticate with CentOps API

* **The scope** of authentication with CentOps API is **service-to-service only,** meaning that any client-side authentication, permissions validation etc is not covered.

* Applications also assume that all incoming requests come from appropriate participants with appropriate rights without any restrictions based on network, certificates etc.

* Service-to-service requests are authenticated by passing on API keys.

* Requests from public network have also access to back-end components, which would otherwise require using Ruuter as a reverse proxy.

* Back-end components can make requests to another back-end components without using Ruuter as a reverse proxy, which would normally be forbidden.

* Service-to-service requests are authenticated by passing on [API keys](https://en.wikipedia.org/wiki/Application_programming_interface_key) via request headers.

### Required functionalities

| Service                                                     | Allowed for | Uses database | Uses API key | Comment                                                                                                         |
| ----------------------------------------------------------- | :---------: | :-----------: | :----------: | --------------------------------------------------------------------------------------------------------------- |
| Request new API key for an Institution                      |      *      |               |              | Should only be for requesting a key, not providing detailed information about Institution or Participant itself |
| Request new API key for a Participant                       | Institution |               |              | Should only be for requesting a key, not providing detailed information about Institution or Participant itself |
| Generate new API key                                        |   CentOps   |               |              | A pure functionality to just generate the API key, will be a target for pen-testers                             |
| Save API key in a database                                  |   CentOps   |    &check;    |              |                                                                                                                 |
| Verify request by matching API key(s) with database entries |   CentOps   |    &check;    |              | _Use multiple keys to verify that Participant matches with Institution?_                                        |

## Registering Institutions & Participants

* Detailed information will be provided by [Updating Institutions and Participants](#updating-institutions-and-participants) appropriate services

| Service                    | Allowed for | Uses database | Uses API key | Comment |
| -------------------------- | :---------: | :-----------: | :----------: | ------- |
| Register a new Institution |   CentOps   |    &check;    |              |         |
| Register a new Participant |   CentOps   |    &check;    |              |         |

## Requesting a list of Participants and Institutions

* This is public information and can be queried by anyone for any number of times

* All cases of misusage will be handled by another components

| Service                        | Allowed for | Uses database | Uses API key | Comment                                                                                                                                                |
| ------------------------------ | :---------: | :-----------: | :----------: | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Request a list of Institutions |      *      |               |              | Additional query parameters may be used but not necessarily in scope of this project                                                                   |
| Provide a list of Institutions |   CentOps   |    &check;    |              |                                                                                                                                                        |
| Request a list of Participants |      *      |               |              | Participants are grouped by their Institutions;<br>Use query parameters to filter by Institutions and/or types of Participants (Chat, Classifier, DMR) |
| Provide a list of Participants |   CentOps   |    &check;    |              |                                                                                                                                                        |

## Updating Institutions and Participants

* Will have a graphical user interface in the future

| Service                                     |     Allowed for      | Uses database | Uses API key | Comment |
| ------------------------------------------- | :------------------: | :-----------: | :----------: | ------- |
| Request current details of an Institution   | Institution, CentOps |               |   &check;    |         |
| Provide current details of an Institution   |       CentOps        |    &check;    |              |         |
| Request to update details of an Institution | Institution, CentOps |    &check;    |              |         |
| Update Institution details                  |       CentOps        |    &check;    |              |         |
| Request current details of a Participant    | Participant, CentOps |               |   &check;    |         |
| Provide current details of a Participant    |       CentOps        |    &check;    |              |         |
| Request to update details of a Participant  | Participant, CentOps |               |   &check;    |         |
| Update Participant details                  |       CentOps        |    &check;    |              |         |

## Participants retrieve a list of other participants from CentOps

| Service                                 | Allowed for | Uses database | Uses API key | Comment                             |
| --------------------------------------- | :---------: | :-----------: | :----------: | ----------------------------------- |
| Generate a list of Participants for DMR |   CentOps   |    &check;    |              | Full list that will be sent to DMRs |
| Send a list of Participants to DMRs     |   CentOps   |               |      ?       | DMR validates CentOps by API key?   |

## Receiving participant logs

| Service                  | Allowed for | Uses database | Uses API key | Comment                           |
| ------------------------ | :---------: | :-----------: | :----------: | --------------------------------- |
| Get logs of Participants |   CentOps   |               |              | Fetch or accept what's been sent? |
