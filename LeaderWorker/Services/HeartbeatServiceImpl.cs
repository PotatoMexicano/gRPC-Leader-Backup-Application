using Grpc.Core;
using GrpcLeaderBackup;

namespace LeaderWorker.Services;

public class HeartbeatServiceImpl : HeartbeatService.HeartbeatServiceBase
{
    private static Timer _timer;

    public override Task<HeartbeatResponse> Check(HeartbeatRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HeartbeatResponse
        {
            IsAlive = true
        });
    }

    public class LeaderState
    {
        public String GuidLeader { get; set; }
    }

    public static void StartLeaderWork(Object state)
    {
        _timer = new Timer(PerformLeaderWork, state, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public static void PerformLeaderWork(Object state)
    {
        LeaderState? leaderState = state as LeaderState;

        Int32[] numbers = GenerateRandomNumbers(20);
        List<Int32> mmcs = numbers.Select(n => CalculateMMC(n, 3)).ToList();

        String guidLeader = leaderState.GuidLeader;
        Console.WriteLine(guidLeader);
    }

    private static Int32[] GenerateRandomNumbers(Int32 count)
    {
        Random random = new Random();
        return Enumerable.Range(0, count).Select(_ => random.Next(1, 100)).ToArray();
    }

    private static Int32 CalculateMMC(Int32 a, Int32 b)
    {
        return Math.Abs(a * b) / CalculateMDC(a, b);
    }

    private static Int32 CalculateMDC(Int32 a, Int32 b)
    {
        while (b != 0)
        {
            Int32 temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}

