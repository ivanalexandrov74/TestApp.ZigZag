using ZigZag.Test;
using ZigZag.Test.Data;

var builder = WebApplication.CreateBuilder(args);

// Add GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddHttpRequestInterceptor<CustomHttpRequestInterceptor>();

// Add services to the container.
builder.Services.AddSingleton<AuthorisationMgr>();

builder.Services.AddControllers();

builder.Services.AddSingleton(provider => new ApiConfig(builder.Configuration));

builder.Services.AddSingleton<Db>();

builder.Services.AddSingleton<ExternalApiContext>();

var app = builder.Build();

app.MapGraphQL();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();

var config = new ApiConfig(builder.Configuration);

new Db(config).CreateDefaultUserIfMissing();

app.Run();
