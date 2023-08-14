using SharpHook;
using SharpHook.Native;

namespace RacialClicker;

/// <summary>
///     Represents an abstract auto-clicker class that simulates mouse clicks at a given rate.
/// </summary>
internal abstract class AutoClicker
{
    private static int _targetCps; // The target clicks per second (CPS).
    private static int _minDelay; // Minimum delay between clicks.
    private static int _maxDelay; // Maximum delay between clicks.
    private static bool _autoClickerEnabled; // Flag to indicate if the auto-clicker is enabled.
    private static readonly EventSimulator Simulator = new(); // Simulates mouse events.
    private static readonly TaskPoolGlobalHook Hook = new(); // Handles global keyboard input.
    private static readonly Random Random = new(); // Generates random numbers.
    private static KeyCode _toggleKey; // Keyboard key to toggle the auto-clicker.
    private static bool _keyHasBeenSet; // Flag to indicate if the toggle key has been set.

    /// <summary>
    ///     The entry point of the application.
    /// </summary>
    private static void Main()
    {
        PrintLabel();

        _targetCps = GetTargetCps(); // Get the target CPS from user input.
        Console.WriteLine("The CPS range is between {0} and {1}.", _targetCps - 2, _targetCps + 2);

        _minDelay = 1000 / (_targetCps + 2); // Calculate minimum delay to achieve CPS range.
        _maxDelay = 1000 / (_targetCps - 2); // Calculate maximum delay to achieve CPS range.

        Hook.KeyPressed += OnKeyPressed; // Subscribe to the global key press event.

        Console.WriteLine("Press any key to set the toggle key...");

        Hook.Run(); // Start the global keyboard hook.
        Hook.Dispose(); // Dispose of the hook when done.
    }

    /// <summary>
    ///     Retrieves the target clicks per second (CPS) from the user.
    /// </summary>
    /// <returns>The target CPS value.</returns>
    private static int GetTargetCps()
    {
        Console.Write("Enter your target CPS: ");
        if (int.TryParse(Console.ReadLine(), out var cps) && cps > 0)
            return cps;

        Console.WriteLine("Invalid input. Using default target CPS of 12.");
        return 12; // Default value
    }

    /// <summary>
    ///     Handles the global key press event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments containing the keyboard input data.</param>
    private static void OnKeyPressed(object? sender, KeyboardHookEventArgs args)
    {
        if (!_keyHasBeenSet) // Check if the toggle key hasn't been set yet.
        {
            _toggleKey = args.Data.KeyCode;
            _keyHasBeenSet = true; // Set the flag indicating that the key has been set.
            Console.WriteLine($"Press {_toggleKey} to toggle the auto clicker.");
        }
        else if (args.Data.KeyCode == _toggleKey) // Toggle the auto-clicker.
        {
            _autoClickerEnabled = !_autoClickerEnabled;

            if (_autoClickerEnabled)
            {
                Console.WriteLine("Auto clicker has been enabled.");
                ThreadPool.QueueUserWorkItem(AutoClickLoop); // Start the auto-click loop.
            }
            else
            {
                Console.WriteLine("Auto clicker has been disabled.");
            }
        }
    }

    /// <summary>
    ///     Simulates the auto-click loop.
    /// </summary>
    /// <param name="state">The state object (unused).</param>
    private static void AutoClickLoop(object? state)
    {
        while (_autoClickerEnabled)
        {
            PerformClick(); // Simulate a mouse click.
            var randomDelay = Random.Next(_minDelay, _maxDelay); // Generate a random delay.
            Thread.Sleep(randomDelay); // Pause execution for the generated delay.
        }
    }

    /// <summary>
    ///     Simulates a mouse click.
    /// </summary>
    private static void PerformClick()
    {
        Simulator.SimulateMousePress(MouseButton.Button1); // Simulate mouse button press.
        Simulator.SimulateMouseRelease(MouseButton.Button1); // Simulate mouse button release.
    }

    /// <summary>
    ///     Prints the application label/logo.
    /// </summary>
    private static void PrintLabel()
    {
        Console.WriteLine("""
                          
                              ____             _       ___________      __
                             / __ \____ ______(_)___ _/ / ____/ (_)____/ /_____  _____
                            / /_/ / __ `/ ___/ / __ `/ / /   / / / ___/ //_/ _ \/ ___/
                           / _, _/ /_/ / /__/ / /_/ / / /___/ / / /__/ ,< /  __/ /
                          /_/ |_|\__,_/\___/_/\__,_/_/\____/_/_/\___/_/|_|\___/_/
                                                                                     
                          
                          """);
    }
}