using System.Text.Json;
using System.Text.Json.Serialization;
using AuthService;
using EventProvider;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(opts => {
                                                     opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                                                     opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                                                     opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                                     opts.AllowInputFormatterExceptionMessages = true;
                                                 });

builder.Logging.AddConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
                                {
                                    logging.LoggingFields = HttpLoggingFields.All;
                                    logging.RequestHeaders.Add("sec-ch-ua");
                                    logging.ResponseHeaders.Add("MyResponseHeader");
                                    logging.MediaTypeOptions.AddText("application/javascript");
                                    logging.RequestBodyLogLimit = 4096;
                                    logging.ResponseBodyLogLimit = 4096;
                                    

                                });
builder.Services.AddSingleton(new Producer("auth"));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHttpLogging();

app.Run();