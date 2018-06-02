## Getting Started with Identity

### from [Pro ASP.NET Core MVC 2](https://www.apress.com/gp/book/9781484231494) by Adam Freeman

Freeman A. (2017) Pro ASP.NET Core MVC 2. Apress, Berkeley, CA



&nbsp;
### Create a starter project

* Start with an empty ASP.NET Core Web Application.
* Include Microsoft.EntityFrameworkCore.Tools.DotNet.
* Edit Startup.
* Create the Controller and View.


&nbsp;
### Set Up ASP.NET Core Identity

* Create the User class which is derived from IdentityUser.
* Configure the View Imports. Add the Users.Models namespace to the view imports file.
* Create the Database Context Class. The context class is derived from IdentityDbContext<T>, where T is the user class (AppUser in the example project).
* Configure the Database Connection String Setting. Use the ASP.NET Configuration File item template to create the appsettings.json file in the root folder of the project.
* Update the Startup class.
  * Add the constructor that takes the configuration parameter. 
  * Configure the services. Add the db context. Add the Identity service.
  * Add an instruction to the app Configure method in order to use authentication.
* Create the database by running `add-migration Initial` and then `update-database` in the Package Manager Console.


&nbsp;
### Use ASP.NET Core Identity

* Enumerate User Accounts.
  * Add the AdminController. 
  * The Index action method enumerates the users managed by the Identity system. Access to the user data is through the `UserManager<AppUser>` object that is received by the controller constructor and provided through dependency injection. 
  * The Users property returns an enumeration of user objects—instances of the AppUser class.
* Create the Views/Admin folder and add a file called Index.cshtml. 
* Create Users. 
  * Add the UserViewModels model.
  * Create the CreateModel class which defines the basic properties required to create a user account.
  * Add a pair of Create action methods to the Admin controller. Use the standard controller pattern to present a view to the user for a GET request and process form data for a POST request.
  * The POST Create action method accepts a CreateModel argument and will be invoked when the administrator submits the form data. Check that the data contains the required values with the ModelState.IsValid property. If it does, a new instance of the AppUser class is created and passed to the asynchronous UserManager.CreateAsync method.
  * The result from the CreateAsync method is an IdentityResult object. The IdentityResult class defines the Succeeded and Errors properties.  
  * If the Succeeded property is true, then the client is redirected to the Index action so that list of users is displayed.
  * If the Succeeded property is false, then the sequence of IdentityError objects provided by the Errors property is enumerated, with the Description property used to create a model-level validation error using the ModelState.AddModelError method.
* Create a view file called Create.cshtml in the Views/Admin folder. 