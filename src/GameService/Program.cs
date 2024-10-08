using GameService.Base;
using GameService.Consumers;
using GameService.Data;
using GameService.Repositories;
using GameService.Repositories.ForCategory;
using GameService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<GameDbContext>(opt =>{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddScoped<IGameRepository,GameRepository>();
builder.Services.AddScoped(typeof(BaseResponseModel));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(opt=>{
    opt.AddConsumersFromNamespaceContaining<GameCreatedFaultConsumer>();

     opt.AddEntityFrameworkOutbox<GameDbContext>(x=>{
        x.QueryDelay = TimeSpan.FromSeconds(10);

        x.UsePostgres();
        x.UseBusOutbox();
     });

    opt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("game",false));
    opt.UsingRabbitMq((context,cfg) =>{
          cfg.Host(builder.Configuration["RabbitMQ:Host"],"/",host =>{
            host.Username(builder.Configuration.GetValue("RabitMQ:Username","guest"));
            host.Username(builder.Configuration.GetValue("RabitMQ:Password","guest"));
          });
          cfg.ConfigureEndpoints(context);
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
