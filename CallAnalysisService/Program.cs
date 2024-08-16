using Microsoft.EntityFrameworkCore;

using CallAnalysisHelper.Database;
using CallAnalysisHelper.Services;
using CallAnalysisHelper.Files;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CallAnalyticsService>();
builder.Services.AddScoped<ExcelCallDataReader>();
builder.Services.AddScoped<CsvClientDataReader>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Создание базы данных и таблиц, если они отсутствуют
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
