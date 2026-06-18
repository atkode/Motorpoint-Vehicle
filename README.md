Ran locally using Swagger UI.

Notes:
- I'm not sure if it was out of scope of the task (or overkill) to add Ids to the vehicles.json, but I thought it was the best way to use the additional HTTP requests I added

Endpoints:
- GET `/vehicles/get-all`  
- GET `/vehicles/get-all/{pageNumber}/{listSize}`  
- GET `/vehicles/{id}`  
- GET `/vehicles/get-make/{make}`  
- GET `/vehicles/get-model/{model}`  
- POST `/vehicles/get-search`  
- POST `/vehicles/create`  
- POST `/vehicles/update`  
- DELETE `/vehicles/delete/{id}`  

Testing
- Unit tests are in Vehicles.Api.xUnit

Known limitations / considerations for future
- Some tests missing - time constraint
- I used soft-delete `IsDeleted` property rather than deleting the vehicle object
- As commented in the interface, realistically I would have paginated all GET requests - not just one
- Put all error strings into a seperate file 
- No authentication/authorization (creating, editing, deleting auths)
