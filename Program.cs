using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Utils.MailUtils;
using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineShoppingCart.Utils;
using OnlineShoppingCart.ExtendMethods;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Mail service
builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);
builder.Services.AddTransient<IEmailSender, SendMailService>();      // Đăng ký dịch vụ Mail
builder.Services.AddScoped<ImageService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddDistributedMemoryCache();
// Đăng ký dịch vụ Session
builder.Services.AddSession(cfg =>
{
    cfg.Cookie.Name = "shoparts";   // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // 30' Thời gian tồn tại của Session
});
// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//connect database
var connectionString = builder.Configuration.GetConnectionString("AppConnectionString") ?? throw new InvalidOperationException("Connection string 'AppConnectionString' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Thêm vào dịch vụ Identity với cấu hình mặc định cho AppUser (model user) vào IdentityRole (model Role)
// Thêm triển khai EF lưu trữ thông tin về Idetity (theo ApplicationDbContext -> MS SQL Server).
// Thêm Token Provider - nó sử dụng để phát sinh token (reset password, confirm email ...)
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true; // Xac thuc new acc truoc khi login
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/access-denied.html";
});


//login from google
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
        googleOptions.CallbackPath = "/login-from-google";

    });



builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<CartService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); //read file from wwwroot

//Tuy bien tao ra noi dung tu error 400-599
app.AddStatusCodePages();

app.UseSession(); //sd Session

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();   // xac thuc thông tin đăng nhập
app.UseAuthorization();   // xac dinh thông tin về quyền của User

app.MapControllerRoute(
    name: "areas",
    pattern: "{areas:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
