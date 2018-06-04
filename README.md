## ASP.NET Core Identity

- CHAPTER 28: Getting Started with Identity
- CHAPTER 29: Applying ASP.NET Core Identity

### from [Pro ASP.NET Core MVC 2](https://www.apress.com/gp/book/9781484231494) by Adam Freeman

Freeman A. (2017) Pro ASP.NET Core MVC 2. Apress, Berkeley, CA

&nbsp;
## CHAPTER 28: Getting Started with Identity

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
* Create the Database Context Class. The context class is derived from `IdentityDbContext<T>`, where T is the user class (AppUser in the example project).
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
* Validate Passwords.
  * The Identity system checks the candidate password and generates errors if it doesn’t match the requirements.  
  * Configure the password validation rules in the Startup class. Specify a minimum length of four characters and disable the other constraints. 
* Implement a Custom Password Validator.
  * The password validation functionality is defined by the `IPasswordValidator<T>` interface in the Microsoft.AspNetCore.Identity namespace, where T is the application-specific user class (AppUser in the example application). 
  * Created the Infrastructure folder and add the CustomPasswordValidator class. The validator class checks to see that the password does not contain the username and that the password does not contain the sequence 12345.   
  * In Startup.cs register the  CustomPasswordValidator class as the password validator for AppUser objects.
  * Also implement a custom validation policy that builds on the foundation provided by the PasswordValidator built-in class that is used by default. Change the custom validator class so that it is derived from PasswordValidator. Now we have a combination of custom and built-in validation.
* Validate User Details.
  * Built-in validation is also performed on usernames and e-mail addresses when accounts are created.
  * Validation can be configured in the Startup class using the IdentityOptions.User property, which returns an instance of the UserOptions class.
* Implement Custom User Validation.
  * The validation functionality is specified by the `IUserValidator<T>` interface, which is defined in the Microsoft.AspNetCore.Identity namespace.
  * In the Infrastructure folder add the CustomUserValidator class. This validator checks the domain of the e-mail address to make sure that it is part of the example.com domain.
  * Register the custom user validator in the Startup.cs file. 
  * The process for combining the built-in validation, which is provided by the `UserValidator<T>` class, with custom validation follows the same pattern as for validating passwords.


&nbsp;
### Complete the Administration Features

* Make changes to the Views/Admin/Index.cshtml file to target the Edit and Delete actions in the Admin controller. 
  * The Delete button posts a form to the Delete action on the Admin controller. 
  * The Edit button is an anchor element that will send a GET request because the first step in the edit process is to display the current data.
  * Add a model validation summary to the view.
* Implement the Delete Feature.
  * The `UserManager<T>` class defines a DeleteAsync method that takes an instance of the user class and removes it from the database. Use the DeleteAsync method to implement the delete feature of the Admin controller.
  * The Delete method receives the unique ID for the user as an argument.
  * Use the FindByIdAsync method to locate the corresponding user object and pass it to DeleteAsync method.
  * The result of the DeleteAsync method is an IdentityResult. Use it to ensure that any errors are displayed to the user.
* Implement the Edit Feature
  * Add support for editing the e-mail address and password for a user account. Add the GET and POST Edit action methods to the Admin controller. 
  * The Edit action targeted by GET requests uses the ID string embedded in the Index view to call the FindByIdAsync method to get an AppUser object that represents the user.
  * The more complex implementation for Edit receives the POST request, with arguments for the user ID, the new e-mail address, and the password. 
  * Validate the received values to ensure that the custom policies for User Details and Passwords are not violated.
  * Add a dependency to the controller constructor for an `IUserValidator<AppUser>` object.
  * Change the value of the Email property because the ValidateAsync method only accepts instances of the user class.
  * ASP.NET Core Identity stores hashes of passwords, rather than the passwords themselves. This is intended to prevent passwords from being stolen.
  * Take the validated password and generate the hash code that will be stored in the database so that the user can be authenticated. Passwords are converted to hashes through an implementation of the `IPasswordHasher<AppUser>` interface, which is obtained by declaring a constructor argument that will be resolved through dependency injection. The  IPasswordHasher interface defines the HashPassword method, which takes a string argument and returns its hashed value.
  * Changes to the user class are stored in the database with a call to the UpdateAsync method.
* Create the View.
  * Create the Edit.cshtml file in the Views/Admin folder. This view displays the user ID, which cannot be changed, as static text and provides a form for editing the e-mail address and password.
* The user class doesn’t contain password information, since only hashed values are stored in the database.
* Comment out the user validation settings from the Startup class so that the default characters for usernames are used. Since some of the accounts in the database were created before the change in the validation setting, you won’t be able to edit them because the usernames won’t pass validation. And since validation is applied to the entire user object when the e-mail address is validated, the result is a user account that cannot be changed.




&nbsp;
## CHAPTER 29: Applying ASP.NET Core Identity


&nbsp;
### Authenticate Users

* The key tool for restricting access to action methods is the Authorize attribute, which tells MVC that only requests from authenticated users should be processed. 
* The ASP.NET Core platform provides information about the user through the HttpContext object, which is used by the Authorize attribute to check the status of the current request and see whether the user has been authenticated. The HttpContext.User property returns an implementation of the IPrincipal interface, which is defined in the System.Security.Principal namespace. 
* The IPrincipal Interface defines among other the following members:
  * Identity: Returns an implementation of the IIdentity interface that describes the user associated with the request.
  * IsInRole(role): Returns true if the user is a member of the specified role.
* The ASP.NET Core Identity middleware uses cookies sent by the browser to determine whether the user has been authenticated. If the user has been authenticated, then the IIdentity.IsAuthenticated property is set to true. 
* If the IsAuthenticated property is false, it causes an authentication error that leads to the client being redirected to the /Account/Login URL, which is the default URL for providing authentication credentials.


* Prepare to Implement Authentication.
  * Apply the Authorize attribute to the Index action of the Home controller.
  * Add the LoginModel class to the UserViewModels.cs file.
  * Add the AccountController to the Controllers folder. Both versions of the Login action method take an argument called returnUrl. When a user requests a restricted URL, they are redirected to the /Account/Login URL with a query string that specifies the URL that the user should be sent back to once they have been authenticated. The value of the returnUrl query string parameter allows navigating between open and secured parts of the application smoothly.
  * Apply the Authorize attribute to the AccountController class and then use the AllowAnonymous attribute on the individual Login action methods. This restricts action methods to authenticated users by default but allows unauthenticated users to log into the application. 
  * Apply the ValidateAntiForgeryToken attribute, which works in conjunction with the form element tag helper to protect against cross-site request forgery.
  * Create the Login view that will be rendered to gather credentials from the user in the Views/Account folder. It contains the hidden input element, which preserves the returnUrl argument. 