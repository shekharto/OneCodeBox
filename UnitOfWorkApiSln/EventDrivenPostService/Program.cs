using EventDrivenPostService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PostService", Version = "v1" });
});




 builder.Services.AddDbContext<PostServiceContext>(options =>
    options.UseSqlite(@"Data Source=post.db"), ServiceLifetime.Transient);

// rabbitmq -- consume// 


var contextOptions = new DbContextOptionsBuilder<PostServiceContext>()
    .UseSqlite(@"Data Source=post.db")
    .Options;

var dbContext = new PostServiceContext(contextOptions);

void ListenForIntegrationEvents()
{
    var factory = new ConnectionFactory();
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
      

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);

        var data = JObject.Parse(message);
        var type = ea.RoutingKey;


        if (type == "user.add")
        {
            dbContext.Posts.Add(new EventDrivenPostService.Model.Post()
            {
                Content = String.Empty,
                Title = String.Empty,
                PostId = data["id"].Value<int>(),
                UserId = data["id"].Value<int>(),
                Name = data["name"].Value<string>(),
            });
            dbContext.SaveChanges();
        }
        else if (type == "user.update")
        {
            var user = dbContext.Posts.First(a => a.UserId == data["id"].Value<int>());
            user.Content = String.Empty;
            user.Title = String.Empty;
            user.PostId = data["id"].Value<int>();
            user.Name = data["newname"].Value<string>();
            dbContext.SaveChanges();
        }


    };
    channel.BasicConsume(queue: "user.postservice",
                             autoAck: true,
                             consumer: consumer);
}


//void ListenForIntegrationEvents()
//{
//    var factory = new ConnectionFactory();
//    var connection = factory.CreateConnection();
//    var channel = connection.CreateModel();
//    var consumer = new EventingBasicConsumer(channel);

//    consumer.Received += (model, ea) =>
//    {

//        var contextOptions = new DbContextOptionsBuilder<PostServiceContext>()
//                 .UseSqlite(@"Data Source=post.db")
//                         .Options;



//        var dbContext = new PostServiceContext(contextOptions);

//        var body = ea.Body.ToArray();
//        var message = Encoding.UTF8.GetString(body);
//        Console.WriteLine(" [x] Received {0}", message);

//        var data = JObject.Parse(message);
//        var type = ea.RoutingKey;

//        if (type == "user.add")
//        {
//            dbContext.Posts.Add(new EventDrivenPostService.Model.Post()
//            {
//                Content = String.Empty,
//                Title = String.Empty,
//                PostId = data["id"].Value<int>(),
//                UserId = data["id"].Value<int>(),
//                Name = data["name"].Value<string>(),
//            });
//            dbContext.SaveChanges();
//        }
//        else if (type == "user.update")
//        {
//            var user = dbContext.Posts.First(a => a.UserId == data["id"].Value<int>());
//            user.Name = data["name"].Value<string>();
//            dbContext.SaveChanges();
//        }




//        //if (type == "user.add")
//        //{
//        //    dbContext.Users.Add(new EventDrivenPostService.Model.User()
//        //    {
//        //        Id = data["id"].Value<int>(),
//        //        Name = data["name"].Value<string>(),
//        //    });
//        //    dbContext.SaveChanges();
//        //}
//        //else if (type == "user.update")
//        //{
//        //    var user = dbContext.Users.First(a => a.Id == data["id"].Value<int>());
//        //    user.Name = data["name"].Value<string>();
//        //    dbContext.SaveChanges();
//        //}
//    };

//    channel.BasicConsume(queue: "user.postservice",
//                autoAck: true,
//                consumer: consumer);
//}

// rabbitmq



var app = builder.Build();
 ListenForIntegrationEvents();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
