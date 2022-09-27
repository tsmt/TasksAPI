using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TasksAPI.Data;
using TasksAPI.Filters;
using TasksAPI.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
string connection = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WebApiContext>(options =>
    options.UseSqlServer(connection), ServiceLifetime.Transient);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SchemaFilter<EnumSchemaFilter>());

// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<TaskFilesService>();
builder.Services.AddScoped<TestTasksService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task API",
        Description = "Task API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Sergey Fedorov",
            Email = string.Empty,
            Url = new Uri("https://google.com"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

string uploadPath = app.Configuration.GetValue<string>("UploadPath");

app.UseFileServer(new FileServerOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), uploadPath)),
    RequestPath = new PathString("/" + uploadPath),
    EnableDirectoryBrowsing = false
});

app.UseAuthorization();

app.MapControllers();

app.Run();
