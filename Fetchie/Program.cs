
using Fetchie.BackgroundServices;
using Fetchie.Queues;
using Fetchie.SignalR.Hubs;

namespace Fetchie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddSingleton<MultiQueueManager>();
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<MultiQueueCleanupService>();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<QueueHub>("/queues");


            app.Run();
        }
    }
}
