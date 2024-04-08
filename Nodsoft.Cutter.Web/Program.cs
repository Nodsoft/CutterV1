namespace Nodsoft.Cutter.Web;

public static class Program
{
	public static async Task Main(string[] args)
	{
		await CreateHostBuilder(args).Build().RunAsync();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
}