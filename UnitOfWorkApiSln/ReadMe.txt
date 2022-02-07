1) in project "UnitOfWork.Api"
it demonstrate how to create better way a generic repository so that common method can implement to all controller repository.
It also demonstrate how to use unit-of-work pattern which help to commit the multiple changes in a single save either complete save or fail,
But not in partial save.

2) in project "EventDrivenPostService" and "EventDrivenUserService" 
This demonstrate how to use the dbconext in better way and create a sql lite db in the project itself.
Important part is by using "RabittMQ" it shows how to update other server database from the one server database using the individual rest web api. like user-service get update the changes will reflect into postuser-service.
