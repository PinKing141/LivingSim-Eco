using LivingSim.Core;

var options = HeadlessOptions.Parse(args);
var simulation = HeadlessWorldRunner.CreatePopulated(options.Settings, options.Herbivores, options.Predators);
simulation.Advance(options.Ticks);

Console.WriteLine($"version={SimulationVersion.Value}");
Console.WriteLine($"seed={options.Settings.Seed} size={options.Settings.Width}x{options.Settings.Height} ticks={simulation.Tick}");
Console.WriteLine($"population herbivores={simulation.Metrics.Herbivores} predators={simulation.Metrics.Predators} carcasses={simulation.Metrics.Carcasses}");
Console.WriteLine($"state={SimulationStateHasher.Hash(simulation)}");

internal sealed record HeadlessOptions(WorldSettings Settings, int Ticks, int Herbivores, int Predators)
{
    public static HeadlessOptions Parse(string[] args)
    {
        var seed = 1;
        var width = 96;
        var height = 64;
        var ticks = 1_000;
        var herbivores = 24;
        var predators = 6;

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
                case "--herbivores": herbivores = value; break;
                case "--predators": predators = value; break;
                default: throw new ArgumentException($"Unknown option {args[index]}.");
            }
        }

        return new HeadlessOptions(new WorldSettings(seed, width, height), ticks, herbivores, predators);
    }
}
