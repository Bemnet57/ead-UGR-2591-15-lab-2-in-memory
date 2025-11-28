using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Data;
using PizzaStore.Models;


var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<PizzaDb>(
    options => options.UseInMemoryDatabase("pizzas")
);

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

// Test endpoint
app.MapGet("/", () => "Hello World!");


//list all items
app.MapGet("/pizzas", async (PizzaDb db) => await 
db.Pizzas.ToListAsync()
);

// the crud points -- get
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await 
db.Pizzas.FindAsync(id));

// The crud points--post
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

//--update
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
      var pizza = await db.Pizzas.FindAsync(id);
      if (pizza is null) return Results.NotFound();
      pizza.Name = updatepizza.Name;
      pizza.Description = updatepizza.Description;
      await db.SaveChangesAsync();
      return Results.NoContent();
});
//--delete

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
   var pizza = await db.Pizzas.FindAsync(id);
   if (pizza is null)
   {
      return Results.NotFound();
   }
   db.Pizzas.Remove(pizza);
   await db.SaveChangesAsync();
   return Results.Ok();
});



app.Run();
// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// app.Run();
