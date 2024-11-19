using Dynq;
using Fetchie.Host.BackgroundServices;
using Fetchie.Host.Components;
using Fetchie.Host.Queues;
using Fetchie.Host.SignalR.Hubs;
using MudBlazor.Services;

namespace Fetchie.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<MessageQueueManager>();
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<MultiQueueCleanupService>();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(Program).Assembly));
            builder.Services.AddDynq([typeof(Program).Assembly]);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("../swagger/v1/swagger.json", "Fetchie");
                options.RoutePrefix = "api";
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapControllers();
            app.MapHub<QueueHub>("/queues");

            app.Run();
        }
    }
}
