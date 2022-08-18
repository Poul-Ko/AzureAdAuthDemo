using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamWebApi("GraphTest", o => o.BaseUrl = "https://graph.microsoft.com")
    .AddInMemoryTokenCaches();

builder.Services.AddCors();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(p =>
{
    p.WithOrigins("http://localhost:5000", "https://localhost:5001");
    p.AllowAnyHeader();
    p.AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();