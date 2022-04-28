# Introduction

Central Operations (CentOps) is a part of the Bürokratt responsible for enrolling and monitoring participants in the Bürokratt network.

It's functionality will include:

1. A CRUD API to support querying, adding, removing or updating Chatbot status.
   * Chatbots will need to be registered with this API in order to take traffic.
2. Mechanisms for monitoring Chatbot health.
3. Mechanisms to communication with Chatbots.
   * In order to notify them to update their components for instance.

## API

APIs will exist to allow Administrators of the Bürokratt platform to add a Chatbot into the Bürokratt network and allow it to take traffic.

These APIs will maintain a data structure for each Chatbot:

| Property         |                  Description                                |                   Possible Values                    |
|------------------|------------------------------------------------------------:|------------------------------------------------------|
| uri              | The endpoint for the specified chatbot                      | 'https://chatbot1:9000', 'https://chatbot2:9000/api' |
| ministry         | The Id of the Ministry the chatbot is handling traffic for  | interior, culture, justice                           |
| status           | The current status of the chatbot                           | 0 (Unknown), 1 (Online), 2 (Offline)                 |
| version          | The current version of the chatbot                          | 1.0.0, 2.1.0, 3.2.1                                  |

* [?]Is this supporting Institution chatbots too - or is that the Ministry chatbots only?
* [?]Instances - we spoke about monitoring individual instances of a ministry chatbot here.  Do you see *uri* being a load balancer address for the ministry endpoint or the address of each individual chatbot for that ministry?)
* [?]Statuses are a 'straw man' here and don't need to be numeric in nature.

### Authentication

CRUD APIs will require authentication to allow them to be contacted.  Likely certain APIs will have separate roles to others...

* For instance enrolment and updating of Chatbot data
* Querying the chatbot store.

* [?]What exactly is this OAuth? Something else?. MTLS? 
* [?]Where are users authenticating?  Do they authenticate interactively or is this something like API keys?

### Scenarios

> TBD
> - [?] can we drill down on this, who is doing what here when it comes to enrolling a chatbot into the system?


## Monitoring

CentOps will 'observe' chatbots for signals they are not operating correctly.  There are various signals emitted by chatbots we can use to determine a Chatbot is having trouble responding to requests.

1. Logging from the Chatbot itself.
2. Calls routed through the DMR will get HTTP response codes.

### Scenarios

> TBD

#### Handling a chatbot which is failing

If enough of an indication is received to indicate the chatbot is having difficulty (e.g. some proportion of operations are failing) monitoring can change the state of the chatbot in the chatbot store to indicate it is currently in a failing state.  This can prevent this chatbot taking traffic and potentially even notify bot owners of the failure.

#### Bringing a chatbot back online

Once the situation has been resolved the administrator can bring the chatbot back online manually by changing the state of the bot using the API.
[?]Is this what we envisage here - or something more automatic perhaps using the [Circuit Breaker Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker#:~:text=The%20Circuit%20Breaker%20pattern%20prevents%20an%20application%20from,to%20invoke%20an%20operation%20through%20a%20circuit%20breaker)?
