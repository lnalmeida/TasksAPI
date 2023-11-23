using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TasksDB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "API online");

app.MapGet("/phrases", async () => await new HttpClient().GetStringAsync("https://api.quotable.io/random"));

app.MapGet("/tasks", async (AppDbContext db) => await db.Tasks.ToListAsync());

app.MapPost("/tasks", async (Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});


app.Run();

class Task
{
    public Task() 
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now.Date;
    }   

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } 
    [Required(ErrorMessage ="The title can not be empty.")]
    public string? Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; } = false;
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Task> Tasks => Set<Task>();
}

