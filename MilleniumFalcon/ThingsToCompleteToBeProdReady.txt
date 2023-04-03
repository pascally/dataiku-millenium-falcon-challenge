## What can be added to the project ?

### Monitoring 
* proper logging system who write into a monitoring System such as Datadog
* proper metrics / health endpoint, expose a Prometheus endpoint for business/Api number of call to be scraped and checked
* add tracing with Opentelemetry

### Setup / Deployment
* solve the file right reading issue when launching the webapi and front end through Docker

### WebApi/Backend
* Rate limiter to the number of Api Calls

### Tests
* Test coverage
* create end-to-end testing 

### Container
* launch frontend + webapi with a docker compose

### Frontend 
* Alert User if problems on file size or invalid input
* Check size of uploaded file