// https://dev.to/francescoxx/c-c-sharp-crud-rest-api-using-net-7-aspnet-entity-framework-postgres-docker-and-docker-compose-493a

using b_robot_api.Authorization;
using b_robot_api.Data;
using b_robot_api.Jobs;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddHttpContextAccessor();

// Added configuration for PostgreSQL
var configuration = builder.Configuration;
builder.Services.AddDbContext<BTaskContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

using(var scope = app.Services.CreateScope())
{
    var btaskContext = scope.ServiceProvider.GetRequiredService<BTaskContext>();

    var btasks = btaskContext.BTasks.ToListAsync();
    foreach (var btask in btasks.Result) {
        var user = await btaskContext.Users.FindAsync(btask.UserId);
        if (user != null) {
            BJob bj = new (btask, user);
            BJobs.AddBJob(bj);
//        bj.Start();
        }
    }
}

app.Run();
