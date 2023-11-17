using AspNetCoreHero.ToastNotification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using ToyStoreOnlineWeb.Data;
using ToyStoreOnlineWeb.Models;
using ToyStoreOnlineWeb.Service;



var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
var connectionString = builder.Configuration.GetConnectionString("ToyStore");
if (connectionString != null)
{
    builder.Services.AddDbContext<ToyStoreDbContext>(options => options.UseSqlServer(connectionString));
}
builder.Services.AddScoped<IFileStorageService, FileStorageService>(sp =>
{
    var hostingEnvironment = sp.GetRequiredService<IWebHostEnvironment>();
    var webRootPath = hostingEnvironment.WebRootPath;
    var fileStoragePath = Path.Combine(webRootPath, "Content/images/"); 

    return new FileStorageService(fileStoragePath);
});
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IAgeService,AgeService>();
builder.Services.AddScoped<IGenderService,GenderService>();
builder.Services.AddScoped<IProducerService, ProducerService>();
builder.Services.AddScoped<IProductCategoryParentService,ProductCategoryParentService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<IDecentralizationService,DecentralizationService>();
builder.Services.AddScoped<IUsersSpinService, UsersSpinService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductViewedService, ProductViewedService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<UnitOfWork>();




builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] {UnicodeRanges.All}));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "SignUp",
        pattern: "dang-ky",
        defaults: new { controller = "Home", action = "SignUp" });

    endpoints.MapControllerRoute(
        name: "SignIn",
        pattern: "dang-nhap",
        defaults: new { controller = "Home", action = "SignIn" });

    endpoints.MapControllerRoute(
        name: "Cart",
        pattern: "gio-hang",
        defaults: new { controller = "Cart", action = "Checkout" });

    endpoints.MapControllerRoute(
        name: "ProductCategoryParent",
        pattern: "danh-muc-goc/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "ProductCategoryParent" });

    endpoints.MapControllerRoute(
        name: "ProductCategory",
        pattern: "danh-muc/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "ProductCategory" });

    endpoints.MapControllerRoute(
        name: "Gender",
        pattern: "gioi-tinh/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "Gender" });

    endpoints.MapControllerRoute(
        name: "Age",
        pattern: "do-tuoi/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "Ages" });

    endpoints.MapControllerRoute(
        name: "Producer",
        pattern: "thuong-hieu/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "Producer" });

    endpoints.MapControllerRoute(
        name: "ProductDetail",
        pattern: "san-pham/{seo-keyword}-{id}",
        defaults: new { controller = "Product", action = "Details" });

    endpoints.MapControllerRoute(
        name: "Product",
        pattern: "san-pham-moi",
        defaults: new { controller = "Product", action = "NewProduct" });


    endpoints.MapControllerRoute(
        name: "Home",
        pattern: "trang-chu",
        defaults: new { controller = "Home", action = "Index" });
}); 
app.Run();
