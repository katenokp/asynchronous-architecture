using BillingService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<BillingCycleRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<TaskRepository>();
builder.Services.AddSingleton<TransactionRepository>();
builder.Services.AddSingleton<BalanceService>();
builder.Services.AddSingleton<BillingService.BillingService>();
builder.Services.AddHostedService<BackgroundExecutor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();