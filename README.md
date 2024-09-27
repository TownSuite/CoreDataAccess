# CoreDataAccess
Core Data Access Utility

## Unit of Work
> The unit of work class serves one purpose: to make sure that when you use multiple repositories,
> they share a single database context. That way, when a unit of work is complete you can call the
> SaveChanges method on that instance of the context and be assured that all related changes will 
> be coordinated.
https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#creating-the-unit-of-work-class
