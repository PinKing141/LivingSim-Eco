using LivingSim.Core;

var options = HeadlessOptions.Parse(args);
var simulation = HeadlessWorldRunner.Create(options.Settings);
simulation.Advance(options.Ticks);

Console.WriteLine($"version={SimulationVersion.Value}");
Console.WriteLine($"seed={options.Settings.Seed} size={options.Settings.Width}x{options.Settings.Height} ticks={simulation.Tick}");
Console.WriteLine($"state={WorldStateHasher.Hash(simulation.World, simulation.Tick)}");

internal sealed record HeadlessOptions(WorldSettings Settings, int Ticks)
{
    public static HeadlessOptions Parse(string[] args)
    {
        var seed = 1;
        var width = 96;
        var height = 64;
        var ticks = 1_000;

        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for {args[index]}.");
            }

            var value = int.Parse(args[index + 1], System.Globalization.CultureInfo.InvariantCulture);
            switch (args[index])
            {
                case "--seed": seed = value; break;
                case "--width": width = value; break;
                case "--height": height = value; break;
                case "--ticks": ticks = value; break;
                default: throw new ArgumentException($"Unknown option {args[index]}.");
            }
        }

        return new HeadlessOptions(new WorldSettings(seed, width, height), ticks);
    }
}
