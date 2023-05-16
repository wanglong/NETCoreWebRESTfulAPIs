# Build a RESTful Web API with ASP.NET Core 6
NETCore6 Web RESTful APIs
In this post, I will demonstrate how to build a RESTful Web API using ASP.NET Core 6.0 and Entity Framework Core.

.NET 6 is the latest LTS (Long Term Support) release currently and will be supported until November 12, 2024.
![image](https://github.com/wanglong/NETCoreWebRESTfulAPIs/assets/316074/fe9097ce-1840-4736-b4a7-a1c4d4c29743)

![image](https://github.com/wanglong/NETCoreWebRESTfulAPIs/assets/316074/e1585ad6-4c2c-4a36-b2ae-d839d01e8082)


This API will manage movie records stored in a relational database (SQL Server) as described in the table below:


The sections of this post will be as follows:

What is REST?
Creating a Web API Project
Adding a Model
Adding a Database Context
Creating Database with Migrations
Creating API Controller and Methods
We need the following tools installed on our computer:

Visual Studio 2022
.NET 6.0 SDK
Microsoft SQL Server Express
If you are ready, let’s get started.

What is REST?
RESTful APIs conform to the REST architectural style.

REST, or REpresentational State Transfer, is an architectural style for providing standards between computer systems on the web, making it easier for systems to communicate with each other.

REST relies on client-server relationship. This essentially means that client application and server application must be able to evolve separately without any dependency on each other.

REST is stateless. That means the communication between the client and the server always contains all the information needed to perform the request. There is no session state in the server, it is kept entirely on the client’s side.

REST provides a uniform interface between components. Resources expose directory structure-like URIs.

REST is not strictly related to HTTP, but it is most commonly associated with it. There are four basic HTTP verbs we use in requests to interact with resources in a REST system:

GET — retrieve a specific resource (by id) or a collection of resources
POST — create a new resource
PUT — update a specific resource (by id)
DELETE — remove a specific resource by id
In a REST system, representations transfer JSON or XML to represent data objects and attributes.

REST has had such a large impact on the Web that it has mostly displaced SOAP-based interface design because it’s a considerably simpler style to use.

Now that we have made a quick review of REST, we can continue with the implementation.

Creating a Web API Project
Open Visual Studio 2022 and select Create a new project and then select ASP.NET Core Web API:


and give a name to your project in the following screen and then click Next.

In the next screen, select .NET 6.0 as the framework and click Create:


At this point we have a starter project as follows:


Default project structure
In the Program.cs we see that Swagger support is added automatically to our project:


Initially created Program.cs
And also Swashbuckle.AspNetCore NuGet package is added as a dependency.

For more information on Swagger, see ASP.NET Core web API documentation with Swagger / OpenAPI.

Now, let’s run (Ctlr+F5) the project to see the default output. When the browser opens and the Swagger UI is shown, select the GET method in the WeatherForecast part and then select Try It Out and Execute:


Swagger UI in browser
Also, you can use the curl URL shown in the Swagger UI for this method and see the result of the URL in the browser:


When we run the application, the default URL comes from the launchSettings.json:


launchSettings.json
And the result values come from the GET method of the WeatherForecastController:


WeatherForecatController.cs
As you see, values here are hard coded and randomness is added to generate different values.

In our Web API, we will create our own records in an SQL server database and will be able to view, update and delete them through REST API endpoints.

Adding a Model
Now, we will implement our data model class.

In Solution Explorer, right-click the project. Select Add -> New Folder and name the folder Models.

Then right-click the Models folder and select Add->Class. Name the class Movie.cs and click Add.

Next, add the following properties to the class:


Movie.cs
The Id field is required by the database for the primary key.

Entity Framework Core
We will use our model with Entity Framework Core (EF Core) to work with a database.

EF Core is an object-relational mapping (ORM) framework that simplifies the data access code. Model classes don’t have any dependency on EF Core. They just define the properties of the data that will be stored in the database.

In this post, we will write the model classes first and EF Core will create the database. This is called Code First Approach.

Let’s add the EF Core NuGet packages to the project. Right-click on the project and select Manage NuGet Packages… and then install the following packages:


NuGet Package Manager
Adding a Database Context
The database context is the main class that coordinates Entity Framework functionality for a data model. This class is created by deriving from the Microsoft.EntityFrameworkCore.DbContext class.

Now, right-click the Models folder and select Add ->Class. Name the class MovieContext and click Add. Then add the following code to the class:


MovieContext.cs
The preceding code creates a DbSet<Movie> property for the entity set.

In Entity Framework terminology, an entity set typically corresponds to a database table and an entity corresponds to a row in the table.

The name of the connection string is passed into the context by calling a method on a DbContextOptions object. For local development, the ASP.NET Core configuration system reads the connection string from the appsettings.json file.

We need to add our connection string to the appsettings.json. I will use the local SQL server instance in my machine and we can define the connection string as follows:


appsettings.json
You can change the database name if you want.

Dependency Injection
ASP.NET Core is built with Dependency Injection (DI). Services (such as the EF Core DB context) are registered with DI during application startup. Components that require these services are provided with these services via constructor parameters.

Now, we will register our database context to the built-in IOC container. Add the following code to Program.cs:


Program.cs
Creating Database with Migrations
Now, we will create the database using the EF Core Migrations feature.

Migrations lets us create a database that matches our data model and update the database schema when our data model changes.

First, we will add an initial Migration.

Open Tools -> NuGet Package Manager > Package Manager Console(PMC) and run the following command in the PMC:

Add-Migration Initial

The Add-Migration command generates code to create the initial database schema which is based on the model specified in the MovieContext class. The Initial argument is the migration name and any name can be used.

After running the command, a migration file is created under the Migrations folder:


Initial migrations file
As the next step, run the following command in the PMC:

Update-Database

The Update-Database command runs the Up method in the Migrations/{time-stamp}_Initial.cs file, which creates the database.

Now, we will check the database created. Open View -> SQL Server Object Explorer.

You will see the newly created database as below:


SQL Server Object Explorer
As you see, the Movie table and the Migrations History table are created automatically. Then a record is inserted into the migration history table to show the executed migrations on the database.

Creating API Controller and Methods
In this section, we will create the Movies API Controller and add the methods to it, and also will test those methods.

Let’s add the controller first. Right-click on the Controller folder and select Add -> Controller.. and then select API Controller - Empty as below:


Click Add and give a name to your controller on the next screen.


MoviesController is created as below:


MoviesController.cs
As you see, the class is decorated with the [ApiController] attribute. This attribute indicates that the controller responds to web API requests.

MoviesController class inherits from ControllerBase.

Next, we will inject the database context mentioned in the previous section through the constructor of the controller. Add the following code:


MoviesController.cs
Now, we will add CRUD (create, read, update, and delete) action methods to the controller. Let’s start with the GET methods.

GET Method
Add the following code to the MoviesController:


GET methods
GetMovies method returns all the movies and GetMovie(int id) method returns the movie having the Id given as input. They are decorated with the [HttpGet] attribute which denotes that a method responds to an HTTP GET request.

These methods implement two GET endpoints:

GET /api/Movies
GET /api/Movies/{id}
We can test the app by calling the two endpoints from a browser as follows:

https://localhost:{port}/api/movies
https://localhost:{port}/api/movies/{id}
The return type of the GetMovie methods is ActionResult<T> type. ASP.NET Core automatically serializes the object to JSON and writes the JSON into the body of the response message. The response code for this return type is 200, assuming there are no unhandled exceptions. Unhandled exceptions are translated into 5xx errors.

Routing and URL Paths

The URL path for each method is constructed as follows:

Start with the template string in the controller’s Route attribute (Route("api/[controller]")). Then replace [controller] with the name of the controller, which by convention is the controller class name minus the Controller suffix. For this sample, the controller class name is MoviesController, so the controller name is movies.

ASP.NET Core routing is case insensitive.

Testing the GetMovie Method

Now we will test these endpoints. Before that, let’s insert some movie records into our table.

Go to the SQL Server Object Explorer and right-click the Movies table and select View Data:


Then add some movie records manually to the table:


We do not need to add data for the Id column as SQL Server automatically handles this for us.

Now, we can test the GET endpoints. Start (Ctlr+F5) the application:


Select the first GET method and click Try it out -> Execute:


This shows all of the movies in the application.

Next, click the second GET method and click Try it out and enter one of the Ids above in the id field and click Execute:


If no item matches the requested Id, the method returns a 404 NotFound error code.


POST Method
Add the following code to the MoviesController:


POST method
PostMovie method creates a movie record in the database. The preceding code is an HTTP POST method, as indicated by the [HttpPost] attribute. The method gets the value of the movie record from the body of the HTTP request.

The CreatedAtAction method:

Returns an HTTP 201 status code, if successful. HTTP 201 is the standard response for an HTTP POST method that creates a new resource on the server.
Adds a Location header to the response. The Location header specifies the URI of the newly created movie record.
References the GetMovie action to create the Location header's URI.
Testing the PostMovie Method

Start the application and then select the POST method in the Movies section.

Click Try it out and enter the movie information that you want to add in the request body:


and click Execute.

Response status code is 201 (Created) and a location header is added to the response as seen below:


We can paste this location URL in the browser and see the response there too:


Also, we can check this record from the Movies table in our local database:


PUT Method
Add the following code to the MoviesController:


PUT method
PutMovie method updates the movie record with the given Id in the database. The preceding code is an HTTP PUT method, as indicated by the [HttpPut] attribute. The method gets the value of the movie record from the body of the HTTP request. You need to supply the Id both in the request URL and the body and they have to match. According to the HTTP specification, a PUT request requires the client to send the entire updated entity, not just the changes.

The response is 204 (No Content) if the operation is successful.

Testing the PutMovie Method

Start the application and then select the PUT method in the Movies section.

Click Try it out and enter the movie information that you want to update in the request body and the Id of the movie in the id field:


and then click Execute.


We can check the updated state of the movie from GET method with Id in the Swagger UI or directly from the browser as below:


We can see the updated info in the database as well:


Movies table
If we try to update a record that does not exist in the database we get 404 Not Found error:


DELETE Method
Add the following code to the MoviesController:


DeleteMovie method deletes the movie record with the given Id in the database. The preceding code is an HTTP DELETE method, as indicated by the [HttpDelete] attribute. This method expects Id in the URL to identify the movie record we want to delete.

Testing the DeleteMovie Method

Start the application and then select the DELETE method in the Movies section.

Click Try it out and enter the Id of the movie you want to delete in the id field:


and then click Execute.


We do not need to supply a request body as you might have noticed. The response status is 204 No Content.

If we try to get this movie record using the browser we get 404 Not Found error as expected:


We can check as well from the database that the record is deleted:


This is where this post ends. You can find the full project in this GitHub repository.

I hope you found this post helpful and easy to follow. Please let me know if you have any corrections and/or questions in the comments below.

And if you liked this post, please clap your hands 👏👏👏

Bye!
