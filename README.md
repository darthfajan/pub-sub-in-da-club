# pub-sub-in-da-club

This repository is though to contain a simple exemple of a Pub/Sub system implementation.

The current implementation is done via a very simple TCP server-client implementation.
The exchange protocol respect the following sequence diagrams:
* ![Subscribe messages exchange](/subscribe.png "Subscribe exchange")
* ![Publish messages exchange](/publish.png "Publish exchange")

An example of utilization can be found in the only **integration test** present in the **PubSub.Tests** project.
You can find also a simple console app for server and client under the **Executables** folder.
* ![Publish messages exchanege](/programExample.png "Publish exchange")

# Build
Download or clone the repo, open the **PubSub.Spike.sln** solution file and build.

# Next steps (a.k.a. things that I whould have done if I had enough time) 
* **Decoupling protocol and communication channel:** currently the BasicTCPServer and the BasicTCPClient send/received strings. That is acceptable but it would be better if an external IContractResolver (as I would call it) is responsible tocheck and convert exchanged bytes into messages based on a specific contract. 

* **Adding new communicationc channels**: it would be good to add new channel of communication, e.g. using Grpc, RabbitMq (with or without Masstransit), Kafka, Redis, Postgres pg_notify and in memory communication.
  
*  **Adding missing unit and integration tests**
