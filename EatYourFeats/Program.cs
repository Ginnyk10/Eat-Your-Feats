/*
 * Prologue
Programmers: Isabel Loney, Anakha Krishna
Date Created: 10/23/2024
Date Revised: 11/10/2024
Purpose: Configures and starts the web application, including services registration, authentication setup, 
and middleware for handling HTTP requests in the EatYourFeats project.

Preconditions: .NET SDK, valid MongoDB connection string, all NuGet packages (MongoDB, cookies, etc.), proper MongoDB connection and authentication setup, all required namespaces
Postconditions: Auth services configured, Razor pages and MVC routing set up, MongoDBService and UserService registered, middleware setup, application runs and listens for requests
Error and exceptions: MongoConfigurationException (incorrect connection string or server unreachable), InvalidOperationException (issues with auth config or middleware)
Side effects: App fails to start or cannot connect to database, users cannot log in or access resources
Invariants: MongoDBService and UserService must always be registered for app to function, UseAuthentication must be called before custom user identity middleware
Other faults: N/A
*/

using EatYourFeats.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Configures the application to use cookie-based authentication with specified login and logout paths
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Login";   // Redirects to /Login if the user is not authenticated
            options.LogoutPath = "/Logout"; // Redirects to /Logout after the user logs out
        });

// Registers Razor Pages, enabling page rendering and MVC-style routing
builder.Services.AddRazorPages();

// Set Splash page as default landing page
builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Splash", "");
});

// MongoDB connection string for connecting to the database
const string connectionUri = "mongodb+srv://jazzy:fierce@eatmyfeats.p8ist.mongodb.net/?retryWrites=true&w=majority&appName=EatMyFeats";

// Registers MongoDBService as a singleton, providing database connectivity throughout the application
builder.Services.AddSingleton(new MongoDBService(connectionUri));

// Registers UserService as a scoped service for handling user-related operations
builder.Services.AddScoped<UserService>();

// Register TaskService and TodoService as a singleton service for handling task-related operations
builder.Services.AddScoped<TodoService>();

builder.Services.AddScoped<GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline, specifying middleware components for request handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Custom error page for non-development environments
    app.UseHsts();                     // Enforces HTTP Strict Transport Security in production environments
}

app.UseHttpsRedirection(); // Redirects all HTTP requests to HTTPS for secure communication
app.UseStaticFiles();      // Enables serving of static files (e.g., CSS, JavaScript)

app.UseRouting();          // Sets up the routing middleware to define endpoints

app.UseAuthentication();   // Enables authentication middleware for managing user login sessions
app.UseAuthorization();    // Enables authorization middleware to restrict access to authenticated users

app.Use(async (context, next) => // Authentication step for landing page
{
    if (context.User.Identity.IsAuthenticated && context.Request.Path == "/") // Conditions to check if user is already registered/login
    {
        context.Response.Redirect("/Dashboard"); // Redirect to dashboard
        return;
    }
    await next(); // Continue
});

app.MapRazorPages();       // Maps Razor Pages endpoints to allow the application to serve pages

app.Run();                 // Starts the application, handling incoming requests
