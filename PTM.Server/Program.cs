using PTM.Infrastructure;
using PTM.Server;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

builder.Services.AddInfrastructureDI(connectionString);


var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.MapScalarApiReference(
        op => op.Servers = []
        );
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
