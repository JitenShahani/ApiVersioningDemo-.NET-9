var builder = WebApplication.CreateBuilder (args);

// Configure IoC/Dependency Container
builder.ConfigureIoCContainer ();

var app = builder.Build ();

// Configure the HTTP request pipeline.
app.ConfigurePipeline ();

// Terminate Middleware and execute the app
app.Run ();