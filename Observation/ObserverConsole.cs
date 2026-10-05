using LivingSim.Core;

namespace LivingSim.Observation;

/// <summary>Temporary terminal presentation; no simulation type depends on this class.</summary>
public static class ObserverConsole
{
    public static void Run(WorldSimulation simulation)
    {
        var controller = new ObserverController();
        controller.State.SetCamera(simulation.World.Width / 2, simulation.World.Height / 2);
        Console.CursorVisible = false;
        try
        {
            while (true)
            {
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.Escape) return;
                    HandleKey(controller, simulation, key);
                }

                controller.Advance(simulation);
                Console.SetCursorPosition(0, 0);
                Console.Write(WorldObserver.Render(simulation, controller.State, controller));
                Console.WriteLine(WorldObserver.Inspector(simulation, controller.State));
                Console.WriteLine(WorldObserver.PopulationPanel(simulation));
                Console.WriteLine(WorldObserver.Graphs(controller.History));
                Console.WriteLine(WorldObserver.Timeline(simulation));
                Console.WriteLine($"zoom={controller.State.ZoomLevel} speed={controller.State.TicksPerFrame} | Space pause | +/- speed | Z/X zoom | arrows camera | Tab overlay | N select | F follow | H latest event | Esc exit");
                Thread.Sleep(50);
            }
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }

    private static void HandleKey(ObserverController controller, WorldSimulation simulation, ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Spacebar: controller.State.TogglePause(); break;
            case ConsoleKey.Add:
            case ConsoleKey.OemPlus: controller.State.SetTicksPerFrame(controller.State.TicksPerFrame * 2); break;
            case ConsoleKey.Subtract:
            case ConsoleKey.OemMinus: controller.State.SetTicksPerFrame(Math.Max(1, controller.State.TicksPerFrame / 2)); break;
            case ConsoleKey.LeftArrow: controller.State.MoveCamera(-3, 0); break;
            case ConsoleKey.RightArrow: controller.State.MoveCamera(3, 0); break;
            case ConsoleKey.UpArrow: controller.State.MoveCamera(0, -3); break;
            case ConsoleKey.DownArrow: controller.State.MoveCamera(0, 3); break;
            case ConsoleKey.Tab: controller.State.CycleOverlay(); break;
            case ConsoleKey.N: controller.SelectNext(simulation); break;
            case ConsoleKey.F: controller.ToggleFollow(); break;
            case ConsoleKey.Z: controller.State.ZoomIn(); break;
            case ConsoleKey.X: controller.State.ZoomOut(); break;
            case ConsoleKey.H:
                if (simulation.NaturalHistory.Events.Count > 0) controller.Focus(simulation.NaturalHistory.Events[^1]);
                break;
        }
    }
}
