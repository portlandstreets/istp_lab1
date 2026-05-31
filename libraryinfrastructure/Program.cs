using libraryinfrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tournamentdomain.Model;
using libraryinfrastructure.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbTournamentContext>(option => option.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));


builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DbTournamentContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Register import/export services and factories
builder.Services.AddScoped<IImportService<Match>, MatchImportService>();
builder.Services.AddScoped<IDataPortServiceFactory<Match>, MatchDataPortServiceFactory>();
builder.Services.AddScoped<IExportService<Match>, MatchExportService>();
builder.Services.AddScoped<MatchDocImportService>();

builder.Services.AddScoped<IExportService<Tournament>, TournamentExportService>();
builder.Services.AddScoped<IDataPortServiceFactory<Tournament>, TournamentDataPortServiceFactory>();

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}")
    //pattern: "{controller=Captains}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
