using Hangfire;
using Hangfire.PostgreSql;
using HangfireWorkerService;

var builder = Host.CreateApplicationBuilder(args);

// Configure Hangfire with PostgreSQL
builder.Services.AddHangfire(configuration => configuration
	.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
	.UseSimpleAssemblyNameTypeSerializer()
	.UseRecommendedSerializerSettings()
	.UsePostgreSqlStorage(options =>
	{
		options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PostgreSqlConnection"));
	}));

// Add Hangfire server
builder.Services.AddHangfireServer();

// Add our custom service
builder.Services.AddScoped<DataService>();

var host = builder.Build();


// Start Hangfire Server
await host.StartAsync();

// Schedule our job to run every minute
RecurringJob.AddOrUpdate<DataService>("ProcessDataJob", service => service.ProcessData(), Cron.Minutely);

host.Run();