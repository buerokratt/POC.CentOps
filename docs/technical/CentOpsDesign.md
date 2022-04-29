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
|------------------|-------------------------------------------------------------|------------------------------------------------------|
| uri              | The endpoint for the specified chatbot                      | 'https://chatbot1:9000', 'https://chatbot2:9000/api' |
| name             | name of the chatbot handling traffic                        | interior, education, education-kesklinna-kool        |
| status           | The current status of the chatbot                           | 'Unknown', 'Online', 'Offline'                       |
| versions         | A collection of chatbot components and their versions       | ```[{ 'name': 'component1', 'version': '1.0.0'}]```  |
| type             | Indication whether this is a ministry bot or institution bot| 'ministry', 'institution'                            |

### Authentication

For service to service communication 

CRUD APIs will require authentication to allow them to be contacted.  Likely certain APIs will have separate roles to others...

APIs which are called by other services will rely on mTLS as a means of authentication.  e.g. DMR calling CentOps to determine the endpoint for a particular Chatbot name.

* [?] do we see this as something Kubernetes will handle for us in a way that is transparent to the service?  Or is this something we specifically need to support in the service itself?

There maybe use cases which require interactive authentication (user based access).  There is a system already in place to use user authentication called TARA which will handle this.

### API Scenarios

#### Onboarding a new Chatbot

* [?] I'd like to understand this scenario much better.  How is this initiated?  Who/What is doing this?

## Monitoring

CentOps will 'observe' chatbots for signals they are not operating correctly.  There are various signals emitted by chatbots we can use to determine a Chatbot is having trouble responding to requests.

1. Logging from the Chatbot itself.
2. Calls routed through the DMR will get HTTP response codes.

Data from Chatbot logs will be stored in CentOps to be analysed to determine how to proceed, either taking the chatbot out of routing or notifying owners of issues.

### Monitoring Scenarios

#### Handling a chatbot which is failing

If enough of an indication is received to indicate the chatbot is having difficulty (e.g. some proportion of operations are failing) monitoring can change the state of the chatbot in the chatbot store to indicate it is currently in a failing state.  This can prevent this chatbot taking traffic and potentially even notify bot owners of the failure.

#### Bringing a chatbot back online

In order to bring a chatbot which is failing back online, a mechanism to test the offline bot with traffic to bring it back 'Online' will be required.

We can look at the [Circuit Breaker Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker#:~:text=The%20Circuit%20Breaker%20pattern%20prevents%20an%20application%20from,to%20invoke%20an%20operation%20through%20a%20circuit%20breaker) for some pointers here.


## Chatbot Maintenance

### Versioning

CentOps will also hold Chatbot component versioning information.  Chatbots themselves can find out if their components are up to date and react accordingly.

* [?] Would this be a notification for Chatbot maintainers or some automatic process?