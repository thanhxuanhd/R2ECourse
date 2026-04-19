using BankAccountSimulationMvc.Interfaces;
using BankAccountSimulationMvc.Models;
using BankAccountSimulationMvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Set default culture to en-GB for consistent currency formatting
var cultureInfo = new System.Globalization.CultureInfo("en-GB");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<DataResourceOptions>(builder.Configuration.GetSection(DataResourceOptions.DataResource));

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<FileService>();

var app = builder.Build();

// Initialize Data Resource
using (var scope = app.Services.CreateScope())
{
    var fileService = scope.ServiceProvider.GetRequiredService<FileService>();
    fileService.Initialize();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();