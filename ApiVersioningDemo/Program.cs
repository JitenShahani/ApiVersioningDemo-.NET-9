var builder = WebApplication.CreateBuilder (args);

// Add services to the container.
IoC.ConfigureIoCContainer (builder);

var app = builder.Build ();

// Configure the HTTP request pipeline.
Middleware.ConfigurePipeline (app);

new MyEndpoints ().MapMyEndpoints (app);

app.Run ();