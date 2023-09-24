using SignalRExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
//builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
//{
//    builder
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .WithOrigins("http://localhost:4200");
//}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

#if !DEBUG
app.UseHttpsRedirection();
#endif


app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});


app.Run();
