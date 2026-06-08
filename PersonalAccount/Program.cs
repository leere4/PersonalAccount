using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data;
using PersonalAccount.Data.Entities;
using PersonalAccount.Mappers;
using PersonalAccount.Models;
using PersonalAccount.Models.Student;
using PersonalAccount.Repository;
using PersonalAccount.Services.Account;
using PersonalAccount.Services.Profile;
using PersonalAccount.Services.Bootstrap;
using PersonalAccount.Services.Smtp;
using PersonalAccount.Services.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
		options.SlidingExpiration = true;
	});
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(
	builder.Configuration.GetConnectionString("SqliteDefaultConnection"))
);

// Options
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Services
builder.Services.AddScoped<IStudentAuthService, StudentAuthService>();
builder.Services.AddScoped<IStudentProfileService, StudentProfileService>();
builder.Services.AddScoped<IStudentService, StudentService>(); 
builder.Services.AddScoped<ISmtpClientService, SmtpClientService>();
builder.Services.AddScoped<IConfirmationTokenService, ConfirmationTokenService>();
if (builder.Environment.IsDevelopment())
	builder.Services.AddScoped<DbBootstrap>();

// Repositories
builder.Services.AddScoped<IStudentRepo<StudentAuthModel>, StudentRepo<StudentAuthModel>>();
builder.Services.AddScoped<IStudentRepo<StudentModel>, StudentRepo<StudentModel>>();
builder.Services.AddScoped<IConfirmationTokenRepo, ConfirmationTokenRepo>();

// Mappers
builder.Services.AddSingleton<IMapper<StudentEntity, StudentAuthModel>, StudentAuthMapper>();
builder.Services.AddSingleton<IMapper<StudentEntity, StudentModel>, StudentMapper>();
builder.Services.AddSingleton<IMapper<ConfirmationTokenEntity, ConfirmationTokenModel>, ConfirmationTokenMapper>();

// Others
builder.Services.AddSingleton<IPasswordHasher<StudentAuthModel>, PasswordHasher<StudentAuthModel>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var bootstrap = scope.ServiceProvider.GetRequiredService<DbBootstrap>();
	await bootstrap.SeedAsync();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();


app.Run();

// Client -> Https -> Server -> Router(FS[Page.cshtml]) -> Page -> PageModel{OnGet/OnPost}
//                    Server -> Page.html + CSS + JS -> Client

// Client -> Https -> Server -> Router(FS[Controller.cs]) -> Controller -> Model
//                                                           Controller -> View
//                    Server -> View + CSS + JS -> Client

// Client -> Https -> Backend -> Controller -> Service -> Repository 
//                               Controller -> JSON -> Client
// Client -> Https -> Frontend
//                    Frontend -> HTML + CSS + JS -> Client