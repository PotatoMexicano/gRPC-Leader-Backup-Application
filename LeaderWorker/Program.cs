using Grpc.Net.Client;
using GrpcLeaderBackup;
using LeaderWorker.Services;

public class BackupService
{
    private readonly String _leaderIp;
    private readonly HttpClient _httpClient;
    private static Boolean _leaderOnline = true;
    public String? GuidLeader;

    public BackupService(String leaderIp)
    {
        _leaderIp = leaderIp;
        _httpClient = new HttpClient();
    }

    public async Task<Boolean> CheckLeader()
    {
        try
        {
            using GrpcChannel channel = GrpcChannel.ForAddress($"http://{_leaderIp}");
            HeartbeatService.HeartbeatServiceClient client = new HeartbeatService.HeartbeatServiceClient(channel);

            Console.Write($"Heartbeat solicitado às {DateTime.Now}: ");

            HeartbeatResponse reply = await client.CheckAsync(new HeartbeatRequest { Message = "Ping" });

            Console.WriteLine(reply.IsAlive ? "Leader" : "");

            return reply.IsAlive;
        }
        catch
        {
            return false;
        }
    }

    private void AssumeLeadership()
    {
        // Executar o trabalho do líder a cada 10 segundos
        while (!_leaderOnline)
        {
            try
            {
                HeartbeatServiceImpl.LeaderState state = new HeartbeatServiceImpl.LeaderState { GuidLeader = GuidLeader };
                HeartbeatServiceImpl.PerformLeaderWork(state);
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();

                _leaderOnline = CheckLeader().Result;
            }
            catch
            {
                throw;
            }
        }
    }

    public async Task MonitorLeader()
    {
        while (true)
        {
            try
            {
                Boolean isLeaderAlive = await CheckLeader();
                _leaderOnline = isLeaderAlive;
                if (!isLeaderAlive)
                {
                    //Console.WriteLine("Líder caiu, assumindo liderança.");
                    AssumeLeadership();
                }
            }
            catch
            {
                //Console.WriteLine("Erro na comunicação com o líder, assumindo liderança.");
                AssumeLeadership();
            }

            await Task.Delay(TimeSpan.FromSeconds(10));

        }

    }

}

public class Program
{
    public static async Task Main(String[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Boolean isLeader = config.GetValue<Boolean>("Leader");
        String leaderIp = config.GetValue<String>("LeaderIp");


        if (isLeader)
        {
            IHost host = CreateHostBuilder(args).Build();

            Console.WriteLine("Iniciando como líder...");
            HeartbeatServiceImpl.LeaderState state = new HeartbeatServiceImpl.LeaderState { GuidLeader = "Leader" };
            HeartbeatServiceImpl.StartLeaderWork(state);
            await host.RunAsync();
        }
        else
        {
            IHost host = CreateHostBuilder(args).Build();

            Console.WriteLine("Iniciando como backup...");

            HeartbeatServiceImpl.LeaderState state = new HeartbeatServiceImpl.LeaderState { GuidLeader = "Backup1" };
            BackupService backupService = new BackupService(leaderIp)
            {
                GuidLeader = state.GuidLeader
            };

            Task monitor = Task.Run(async () =>
            {
                await backupService.MonitorLeader();
            });

            Task server = Task.Run(async () =>
            {
                await host.RunAsync();
            });

            await Task.WhenAll(monitor, server);

        }
    }

    public static IHostBuilder CreateHostBuilder(String[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseUrls("http://*:0");
        webBuilder.UseStartup<Startup>();
    });
    }
}

public class Startup()
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<HeartbeatServiceImpl>();
        });
    }
}
