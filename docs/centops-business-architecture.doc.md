# CentOps business architecture

This document outlines the business architecture of the CentOps component.

## Introduction

CentOps is a component to manage all of the Buerokratt's ecosystem's central operations related to all of its participants.

## Limitations

CentOps is one of the few components being in total control of Buerokratt's core team with ecosystem's participants being only able to consume its services having no other control over it.

## Features

### Institutions

*Ministries etc*

| Service                                                           |  Requests from  | Priority |
|-------------------------------------------------------------------|:---------------:|:--------:|
| Get a list of all registered institutions                         |        *        |          |
| Register a new institution                                        | Buerokratt Core |          |
| Get detailed information about a specific institution             |        *        |          |
| Update detailed information of a specific institution             | Buerokratt Core |          |
| Delete specific institution                                       | Buerokratt Core |          |

### Participants

*Technical components*

| Service                                                           |         Requests from         | Priority |
|-------------------------------------------------------------------|:-----------------------------:|:--------:|
| Get a list of all registered participants grouped by institutions |               *               |          |
| Get a list of all registered participants of an institution       |               *               |          |
| Get detailed information about a specific participant             |               *               |          |
| Request registration of a new participant                         | Institutions, Buerokratt Core |          |
| Register a new participant for an institution                     |        Buerokratt Core        |          |
| Request updating detailed information of a specific participant   | Institutions, Buerokratt Core |          |
| Update detailed information of a specific participant             |        Buerokratt Core        |          |
| Request suspending a specific participant                         | Institutions, Buerokratt Core |          |
| Suspending specific participant                                   |        Buerokratt Core        |          |
| Request revoking a specific participant                           | Institutions, Buerokratt Core |          |
| Revoke specific participant                                       |        Buerokratt Core        |          |
| Delete specific institution                                       |        Buerokratt Core        |          |

### Updating The List of Participants Of Buerokratt Ecosystem

| Service                                                |  Requests from  | Priority |
|--------------------------------------------------------|:---------------:|:--------:|
| Create a list of up-to-date Participants               | Buerokratt Core |          |
| Send a list of up-to-date Participants to Participants | Buerokratt Core |          |

### Software updates

| Service                                              |  Requests from  | Priority |
|------------------------------------------------------|:---------------:|:--------:|
| Notify Participants about optional software updates  | Buerokratt Core |          |
| Notify Participants about mandatory software updates | Buerokratt Core |          |
| Notify Participants about critical software updates  | Buerokratt Core |          |
| Initiate pipeline(s) to update software              |   Participants  |          |

### Health information

*Based on collected logs*

| Service                                                                | Requests from | Priority |
|------------------------------------------------------------------------|:-------------:|:--------:|
| Get current health information of all registered participants          |       *       |          |
| Get historical health information of all registered participants       |       *       |          |
| Get current health information of a specific registered participant    |       *       |          |
| Get historical health information of a specific registered participant |       *       |          |

### Outages

| Service                                                                 |         Requests from         | Priority |
|-------------------------------------------------------------------------|:-----------------------------:|:--------:|
| Get a list of planned outages of all registered participants            |               *               |          |
| Inform about a planned outage of all components of an Institution       | Institutions, Buerokratt Core |          |
| Inform about a planned outage of a specific component of an Institution | Institutions, Buerokratt Core |          |
| Inform about changes in planned outages                                 | Institutions, Buerokratt Core |          |
| Update information about outages                                        |        Buerokratt Core        |          |
| Publish information about outages                                       |        Buerokratt Core        |          |
| Notify participants about outages                                       |        Buerokratt Core        |          |

### Monitoring

| Service                                                             |  Requests from  | Priority |
|---------------------------------------------------------------------|:---------------:|:--------:|
| Send aggregated logs to central monitoring                          |   Institutions  |          |
| Accept logs from Participants                                       | Buerokratt Core |          |
| Verify the validity of logs of Participants                         | Buerokratt Core |          |
| Detect Participants not sending logs                                |   Institutions  |          |
| Process logs of Participants                                        | Buerokratt Core |          |
| Duplicate log entries with detected problems for further processing | Buerokratt Core |          |

### Certificates

| Service                                    |  Requests from  | Priority |
|--------------------------------------------|:---------------:|:--------:|
| Request a new certificate for Participant  |   Institutions  |          |
| Generate a new certificate for Participant | Buerokratt Core |          |
| Notify Participants about new certificates | Buerokratt Core |          |
| Apply new certificates by Participants     | Buerokratt Core |          |
