var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<WordleSolver.Services.WordDictionaryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;
    if (host.Contains("herokuapp.com"))
    {
        var withDomain = "https://www.wordlewizard.com" + context.Request.Path + context.Request.QueryString;
        context.Response.Redirect(withDomain, permanent: true);
    }
    else
    {
        await next();
    }
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();