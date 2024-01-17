using RedisRepository1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var configuration = builder.Configuration;

 string hostName = configuration.GetSection("Redis")["HostName"];
string port = configuration.GetSection("Redis")["Port"]; 
string password = configuration.GetSection("Redis")["Password"]; 
var defaultdatabase = Convert.ToInt32(configuration.GetSection("Redis")["Defaultdatabase"]);
//redis
builder.Services.AddRedis(options =>
{
    options.HostName = hostName;
    options.Password = password;
    options.Port = port;
    options.Defaultdatabase = defaultdatabase;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
