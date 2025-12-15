
using System.Diagnostics;
using System.Reflection;


public class Program
{

    public static async Task Main(string[] args)
    {
        await MakeCoffee();
    }

    public static async Task MakeCoffee()
    {
        var cm = new CoffeeMaker();

        var turnOnTask = cm.TurnOn();
        var waterTask = MakeWaterReady(cm); // AddWater -> HeatWater
        var beansTask = MakeBeansReady(cm); // ScoopBeans -> LoadBeans

        await Task.WhenAll(turnOnTask, waterTask, beansTask);

        await cm.BrewCoffee();
        await cm.PourCoffee();

        await Task.WhenAll(cm.ClearBeans(), cm.TurnOff());
    }

    private static async Task MakeWaterReady(CoffeeMaker cm)
    {
        await cm.AddWater();
        await cm.HeatWater();
    }

    private static async Task MakeBeansReady(CoffeeMaker cm)
    {
        await cm.ScoopBeans();
        await cm.LoadBeans();
    }

    private static DateTime GetBuildTimeUtc()
        => System.IO.File.GetLastWriteTimeUtc(Assembly.GetExecutingAssembly().Location);
}

public static class ActivityMs
{
    public const int TurnOnCoffeeMaker = 1000;
    public const int AddWater = 5000;
    public const int ScoopBeans = 2000;
    public const int LoadBeans = 5000;
    public const int HeatWater = 5000;
    public const int BrewCoffee = 10000;
    public const int PourCoffee = 5000;
    public const int ClearBeans = 2000;
    public const int TurnOffCoffeeMaker = 500;
}

public class CoffeeMaker
{
    private static readonly Stopwatch Proc = Stopwatch.StartNew();
    private static string Now() => Proc.Elapsed.ToString(@"hh\:mm\:ss\.fff");
    private static int Tid() => Thread.CurrentThread.ManagedThreadId;

    private async Task DoSomething(string name, int waitMs)
    {
        Console.WriteLine($"[{Now()}] [T{Tid()}] START  {name,-18} (wait {waitMs} ms)");

        var sw = Stopwatch.StartNew();
        await Task.Delay(waitMs);
        sw.Stop();

        Console.WriteLine($"[{Now()}] [T{Tid()}] END    {name,-18} (waited {sw.ElapsedMilliseconds} ms)");
    }

    public Task TurnOn() => DoSomething(nameof(ActivityMs.TurnOnCoffeeMaker), ActivityMs.TurnOnCoffeeMaker);

    public Task AddWater() => DoSomething(nameof(ActivityMs.AddWater), ActivityMs.AddWater);
    public Task HeatWater() => DoSomething(nameof(ActivityMs.HeatWater), ActivityMs.HeatWater);

    public Task ScoopBeans() => DoSomething(nameof(ActivityMs.ScoopBeans), ActivityMs.ScoopBeans);
    public Task LoadBeans() => DoSomething(nameof(ActivityMs.LoadBeans), ActivityMs.LoadBeans);

    public Task BrewCoffee() => DoSomething(nameof(ActivityMs.BrewCoffee), ActivityMs.BrewCoffee);
    public Task PourCoffee() => DoSomething(nameof(ActivityMs.PourCoffee), ActivityMs.PourCoffee);

    public Task ClearBeans() => DoSomething(nameof(ActivityMs.ClearBeans), ActivityMs.ClearBeans);
    public Task TurnOff() => DoSomething(nameof(ActivityMs.TurnOffCoffeeMaker), ActivityMs.TurnOffCoffeeMaker);
}
