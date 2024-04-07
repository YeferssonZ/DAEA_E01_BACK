using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Conexión a la base de datos MongoDB
var connectionString = "mongodb+srv://zaph:lhnnNWCOoC1jqLbQ@cluster0.awt8ftp.mongodb.net/tareasDB";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("tareasDB");

builder.Services.AddSingleton(database);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapGet("/api/tareas", async (IMongoDatabase database) =>
{
    var collection = database.GetCollection<BsonDocument>("tareas");
    var tareas = await collection.Find(new BsonDocument()).ToListAsync();

    var tareasJson = tareas.Select(t => t.ToDictionary());

    return Results.Json(tareasJson);
});

// Insertar las tareas en la base de datos al iniciar la aplicación
var collection = database.GetCollection<BsonDocument>("tareas");
var tareasNuevas = new List<BsonDocument>
{
    new BsonDocument
    {
        { "nombre", "Comprar víveres" },
        { "descripcion", "Ir al supermercado y comprar lo necesario para la semana" },
        { "completada", false }
    },
    new BsonDocument
    {
        { "nombre", "Hacer ejercicio" },
        { "descripcion", "Realizar una rutina de ejercicios en casa durante 30 minutos" },
        { "completada", true }
    }
};

await collection.InsertManyAsync(tareasNuevas);

Console.WriteLine("Tareas agregadas correctamente.");

// Habilitar CORS
app.UseCors();

app.Run();
