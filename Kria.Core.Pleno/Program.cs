using Kria.Core.Pleno;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Injection();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
