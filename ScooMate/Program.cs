using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using ScooMate.Data;
using System.Globalization;
using ScooMate.Services;

var builder = WebApplication.CreateBuilder(args);

// türkçe kültür ayarını oluştur
var turkishCulture = new CultureInfo("tr-TR");
turkishCulture.NumberFormat.NumberDecimalSeparator = ",";
turkishCulture.NumberFormat.CurrencyDecimalSeparator = ",";

// Thread kültürünü ayarla
CultureInfo.DefaultThreadCurrentCulture = turkishCulture;
CultureInfo.DefaultThreadCurrentUICulture = turkishCulture;

// Add the appsettings.json configuration file
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddHostedService<BildirimKontrolService>();
builder.Services.AddHostedService<AylikSifirlayiciService>();
builder.Services.AddDbContext<ScoomateContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."))
    .ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Bu alan zorunludur.");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(_ => "Geçersiz değer.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(_ => "Lütfen geçerli bir sayı giriniz.");
})
.AddViewOptions(options =>
{
    options.HtmlHelperOptions.Html5DateRenderingMode = Microsoft.AspNetCore.Mvc.Rendering.Html5DateRenderingMode.Rfc3339;
})
.AddDataAnnotationsLocalization();

// Localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(turkishCulture);
    options.SupportedCultures = new List<CultureInfo> { turkishCulture };
    options.SupportedUICultures = new List<CultureInfo> { turkishCulture };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // Enable session middleware

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database and tables are created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScoomateContext>();
    try
    {
        // Veritabanı bağlantısını test et
        context.Database.CanConnect();
        Console.WriteLine("Veritabanı bağlantısı başarılı!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Veritabanı bağlantı hatası: {ex.Message}");
    }
    context.Database.EnsureCreated();
    context.EnsureTablesCreated();
}

// kültür ayarını middleware olarak ekle
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(turkishCulture),
    SupportedCultures = new List<CultureInfo> { turkishCulture },
    SupportedUICultures = new List<CultureInfo> { turkishCulture }
});

app.Run();
