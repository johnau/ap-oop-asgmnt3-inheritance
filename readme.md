# C# OOP Assignment 3: Task Manager: Inheritance (branch "master")
# C# OOP Assignment 4: Task Manager: Binary Files (branch "part2")
# C# OOP Assignment 5: Task Manager: Indexing (branch "part3")
# C# OOP Assignment 6: Task Manager: Documentation (branch "part4")

## Tests
- TaskManagerCore.XunitTests Package contains Unit tests for the relevant classes
- TaskManagerConsoleApp package contains an 'integration' test

## Concepts explored
- Domain Driven Design (kind of); Service/Controller layer -> Infrastructure layer -> Domain/Model layer.  Separation through interfaces that could be improved with CDI/IoC. 
- Repository + DAO combination that would allow better disconnection after IoC is implemented
- DTOs; looking at how to structure DTOs with the inheritance hierarchy of this assignment
- Immutable Builder pattern in Model classes
- Inheritance in testing -  Extending classes and implementing interfaces to create mock objects for tests.  Attempting to use inheritance in testing to avoid creating minimal excess to the classes
- Encapsulation in regards to tests - standard practice seems to be a separate assembly for tests - internal classes can be accessed with `[assembly: InternalsVisibleTo("TestingProject.Tests")]`
- Writing C# 7.3 for .Net framework 4.8 (have been working with the conveniences of C#11+ up until now)
- Casting, implcit and explicit (coercion), `a.GetType() == typeof(Clazz)`, `is`, `as`, is with explicit cast `(a is Clazz b)`, user-defined conversion operators (static implicit/explicit operator extension methods)

## Requirements
- Implement `Task` object (Must inlcude immutable GUID Id, description, notes, completed flag, optional due date, overdue calculation)
-- The TaskData object is implemented as an Immutable class in the Model layer, and as a mutable entity class (besides Id) in the Infrastructure layer.  The Id is protected from mutation inside the infrastructure layer since access to the package from outside is through a domain layer interface, obfuscating/decoupling persistence implementation behind.
- Implement repeating and repeating with streaks sub-classes
-- Similar to above
- Implement Folders (with Name and List<string> of Task Id's, Total of Incomplete tasks in folder, add task method, remove task method) for tasks to be viewed through / grouped by.  
-- The Total Count of Incomplete tasks in the folder has been shifted up the hierarchy and is currently handled at the controller level. (Interpreting the brief, it seems like the calculation is supposed to exist in the Folder class, however this would add some weight to what is currently a lightweight class.
- Implement static database; to be switched out for real database
-- Started out with static holders, have switched around a bit to explore some various ideas/structures around the infrastructure/persistence layer.

![Test Listing](https://github.com/johnau/ap-oop-asgmnt3-inheritance/blob/master/images/test_listing.png?raw=true)
