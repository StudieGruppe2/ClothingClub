using Backend.Repositories;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IClubMemberRepository, ClubMemberRepositoryMemory>();
//Skift fra Memory til MongoDB, når databasen skal bruges
//builder.Services.AddSingleton<IClubMemberRepository, ClubMemberRepositoryMongoDB>();


builder.Services.AddCors(OpsaetCors);

var app = builder.Build();

app.UseCors();

app.MapControllers();

app.Run();

static void OpsaetCors(CorsOptions options)
{
    CorsPolicy policy = new CorsPolicyBuilder()
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .Build();

    options.AddDefaultPolicy(policy);
}