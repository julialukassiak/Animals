using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var animals = new List<Animal>();
var visits = new List<Visit>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var animalsDictionary = new Dictionary<int, Animal>();
var visitsDictionary = new Dictionary<int, Visit>();
int animalIdCounter = 1;
int visitIdCounter = 1;

app.MapGet("ANIMALS", () => Results.Ok(animalsDictionary.Values.ToList()))
    .WithName("GetAllAnimals");



app.MapGet("ANIMALS_ID", (int id) =>
    {
        if (animalsDictionary.TryGetValue(id, out Animal animal))
        {
            return Results.Ok(animal);
        }
        return Results.NotFound($"Animal with id {id} not found.");
    })
    .WithName("GetAnimalById");



app.MapPost("ANIMALS", (Animal animal) =>
    {
        animal.Id = animalIdCounter++;
        animalsDictionary.Add(animal.Id, animal);
        return Results.Created($"/Animals/{animal.Id}", animal);
    })
    .WithName("AddAnimal");



app.MapPut("ANIMALS_ID", (int id, Animal animal) =>
    {
        if (animalsDictionary.ContainsKey(id))
        {
            animalsDictionary[id] = animal;
            return Results.Ok(animal);
        }
        return Results.NotFound($"Animal with id {id} not found.");
    })
    .WithName("UpdateAnimal");



app.MapDelete("ANIMALS_ID", (int id) =>
    {
        if (animalsDictionary.ContainsKey(id))
        {
            animalsDictionary.Remove(id);
            return Results.Ok($"Animal with id {id} removed.");
        }
        return Results.NotFound($"Animal with id {id} not found.");
    })
    .WithName("DeleteAnimal");



app.MapGet("ANIMALS_ID_VISITS", (int id) =>
    {
        var animalVisits = visitsDictionary.Values.Where(v => v.AnimalId == id).ToList();
        return Results.Ok(animalVisits);
    })
    .WithName("GetVisitsByAnimalId");



app.MapPost("VISITS", (Visit visit) =>
    {
        visit.Id = visitIdCounter++;
        visitsDictionary.Add(visit.Id, visit);
        return Results.Created($"/Visits/{visit.Id}", visit);
    })
    .WithName("AddVisit");



app.Run();


public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Weight { get; set; }
    public string FurColor { get; set; }
}

public class Visit
{
    public int Id { get; set; }
    public DateTime VisitDate { get; set; }
    public int AnimalId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
