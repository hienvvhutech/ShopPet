using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Shopping_Pet.Data;
using Shopping_Pet.Helpers;
using Shopping_Pet.Hubs;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;
using Shopping_Pet.Repositories.Implementations;
using Shopping_Pet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[AuthDebug] Auth Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var loggerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("RequestLogger");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOrAdmin", policy => policy.RequireRole("Employee", "Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Staff", "Admin"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AddressService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/chatHub"))
    {
        Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path} from {context.Connection.RemoteIpAddress}");
    }
    await next();
});

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Starting application and running Seed...");

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        await context.Database.MigrateAsync();
    }

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var seedLogger = services.GetRequiredService<ILogger<Program>>();
    await SeedData.InitializeAsync(context, userManager, roleManager, logger);
}
catch (Exception ex)
{
    startupLogger.LogError(ex, "Error seeding database.");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
