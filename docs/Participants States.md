# Participants States

This document defines the states of all participants of the Buerokratt network. The states will be stored, updated and removed as part of the CentOps Admin operations to ensure that the overall state of the network is kept up to date and concurrent with the literal state of the institutions participating in the Buerokratt network.  


<u>**Term definition**:</u> Participant

Is any component operating in the Beurokratt network. These are: Bots, Classifier and DMRs



## Bot States

| Scenario                                                     | State                          |
| ------------------------------------------------------------ | ------------------------------ |
| Institution requests to be registered into the Buerokratt network, the institution gets added to the routing table: Institution | Institution: Offline           |

| Institution confirms the Bot is live and ready to accept messages: | Institution: Offline -> Online |
| Institution makes a request to go offline                    | Institution: Online -> Offline |
| Request for institution to be removed from the Beurokratt Network | Institution: Revoked           |
| Request for institution bot to be deleted from the Beurokratt Network | Institution: Deleted           |



## Classifier States

| Scenario                                       | State                         |
| ---------------------------------------------- | ----------------------------- |
| Classifier Deployed                            | Classifier: Offline           |
| Classifier Operational                         | Classifier: Offline -> Online |
| Classifier to be taken offline for maintenance | Classifier: Online -> Offline |
| Classifier to be deleted from the network      | Classifier: Deleted           |



## DMR States

| Scenario                                | State                  |
| --------------------------------------- | ---------------------- |
| DMR Deployed                            | DMR: Offline           |
| DMR Operational                         | DMR: Offline -> Online |
| DMR to be taken offline for maintenance | DMR: Online -> Offline |
| DMR to be deleted from the network      | DMR: Deleted           |