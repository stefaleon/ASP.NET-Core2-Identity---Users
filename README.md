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
  
* Add User Authentication.
  * Requests for protected action methods are being correctly redirected to the Account controller, but the credentials provided by the user are not yet used for authentication. 
  * In the AccountController, complete the implementation of the Login action, using ASP.NET Core Identity services to authenticate the user against the details held in the database.
  * Get the AppUser object that represents the user through the FindByEmailAsync method of the UserManager<AppUser> class. 
  * If there is an account with the e-mail address that the user has specified, then the next step is to perform the authentication step, which is done using the SignInManager<AppUser> class. Add a constructor argument that will be resolved using dependency injection. Use the SignInManager class to perform two authentication steps.
  * The SignOutAsync method cancels any existing session that the user has, and the PasswordSignIn method performs the authentication. The arguments for the PasswordSignInAsync method are the user object, the password that the user has provided, a bool argument that controls whether the authentication cookie is persistent (disabled) and whether the account should be locked out if the password is correct (disabled).
  * The result of the PasswordSignInAsync method is a SignInResult object, which defines a bool Succeeded property that indicates if the authentication process has been successful. Check the Succeeded property and redirect the user to the returnUrl location if it is true or add a validation error and redisplay the Login view.
  * As part of the authentication process, Identity adds a cookie to the response, which the browser then includes in any subsequent request and which is used to identify the user’s session and the account that is associated with it. It is handled automatically by the Identity middleware.


&nbsp;
### Authorize Users with Roles

* The Authorize attribute is currently used in its most basic form, which allows any authenticated user to execute the action method. It can also be used to refine authorization to give fine-grained control over which users can perform which actions, based on a user’s membership of a role.
* A role is just an arbitrary label that you define to represent permission to perform a set of activities within an application. Almost every application differentiates between users who can perform administration functions and those who cannot. In the world of roles, this is done by creating an Administrators role and assigning users to it.
* Users can belong to many roles, and the permissions associated with roles can be as coarse or as granular as you like, so you can use separate roles to differentiate between administrators who can perform basic tasks, such as creating new accounts, and those who can perform more sensitive operations, such as accessing payment data.
* ASP.NET Core Identity takes responsibility for managing the set of roles defined in the application and keeping track of which users are members of each one. But it has no knowledge of what each role means; that information is contained within the MVC part of the application, where access to action methods is restricted based on role membership.
* ASP.NET Core Identity provides a strongly typed base class for accessing and managing roles called `RoleManager<T>`, where T is the class that represents roles in the storage mechanism. Entity Framework Core
uses a class called IdentityRole to represent roles. It defines the following properties:
  * Id: Defines the unique identifier for the role
  * Name: Defines the name of the role
  * Users: Returns a collection of IdentityUserRole objects that represent the members of the role
* ASP.NET Core Identity is already instructed to use IdentityRole to represent roles with the AddIdentity call in the ConfigureServices method of the Startup class. The AppUser class is used to represent users, and the built-in
IdentityRole class is used for roles.

* Create and Delete Roles
  * Create an administration tool for managing roles. Create action methods that can create and delete roles. In the Controllers folder add the RoleAdminController controller.
  * Roles are managed using the `RoleManager<T>` class, where T is the type being used to represent roles (the built-in IdentityRole class for this application). The RoleAdminController constructor declares a constructor dependency on RoleManager<IdentityRole>, which is resolved using dependency injection when the controller is created.
  * The `RoleManager<T>` class defines the following methods and properties, which allow roles to be created and managed.
    * CreateAsync(role): Creates a new role
    * DeleteAsync(role): Deletes the specified role
    * FindByIdAsync(id): Finds a role by its ID
    * FindByNameAsync(name): Finds a role by its name
    * RoleExistsAsync(name): Returns true if a role with the specified name exists
    * UpdateAsync(role): Stores changes to the specified role
    * Roles: Returns an enumeration of the roles that have been defined
  * The new controller’s Index action method displays all the roles in the application.
  * The Create action methods are used to display and receive a form, the data from which is used to create a new role using the CreateAsync method.
  * The Delete action method receives a POST request and receives the unique ID of a role, which is used to remove it from the application using the DeleteAsync method, having located the object that represents it using the FindByIdAsync method.
 
* Create the Views.   
  * Add a using statement for Microsoft.AspNetCore.Identity to the _ViewImports.cshtml file.
  * Create the Views/RoleAdmin folder and add the Index.cshtml file.
  * This view uses a table to display details of the roles in the application. The third column uses the identity-role custom element attribute.
  * Displaying a list of the users who are members of each role, requires too much code to be included in a view. Add a class file called RoleUsersTagHelper.cs to the Infrastructure folder and use it to define the RoleUsersTagHelper tag helper.
  * This tag helper operates on td elements with an identity-role attribute, which is used to receive the name of the role that is being processed. The `RoleManager<IdentityRole>` and `UserManager<AppUser>` objects allow queries of the Identity database to build up a list of usernames in the role. 
  * Add the tag helper t to the _ViewImports.cshtml file.
  * Add the Create.cshtml view to the Views/RoleAdmin folder to support adding new roles.
  * The only form data needed to create a role is the name. Use a string as the view model class in the Create.cshtml view.
  * Validate that the user supplies a value when the form is submitted. In the Create POST method in RoleAdminController, apply the Required validation attribute directly to the parameter.