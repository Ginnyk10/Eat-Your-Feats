using EatYourFeats.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register MongoDbService with connection string
const string connectionUri = "mongodb+srv://21gke10:<Gk7856212727$>@eatmyfeats.p8ist.mongodb.net/?retryWrites=true&w=majority&appName=EatMyFeats";
builder.Services.AddSingleton(new MongoDBService(connectionUri));


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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
