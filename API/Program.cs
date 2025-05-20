using API;
using Base;
using CompanyServices;
using Interfaces;
using ACIA.Data;
using ACIA.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add controllers with localization for data annotations
builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));
// Register services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IInsuredCompanyService, InsuredCompanyService>();
builder.Services.AddScoped<IIdPoolService, IdPoolService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:4200") // Angular app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("he-IL")

};

// Configure the localization options
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        // Order matters here - this will check headers first
        new AcceptLanguageHeaderRequestCultureProvider(),
        new QueryStringRequestCultureProvider() {QueryStringKey = "lang"},
        new CookieRequestCultureProvider()
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Create database if it doesn't exist
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();