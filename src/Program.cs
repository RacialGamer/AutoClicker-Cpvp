using System.Diagnostics;
using SharpHook;
using SharpHook.Native;

namespace RacialClicker;

/// <summary>
///     Represents an abstract auto clicker that simulates a human-like clicking pattern.
/// </summary>
internal abstract class AutoClicker
{
    private static int _targetCps;
    private static bool _autoClickerEnabled;
    private static readonly EventSimulator Simulator = new();
    private static readonly TaskPoolGlobalHook Hook = new();
    private static KeyCode _toggleKey;
    private static bool _keyHasBeenSet;
    private static Timer _clickTimer = null!;
    private static readonly Stopwatch ClickStopwatch = new();
    private static readonly Random Random = new();
    private static readonly object ClickingLock = new(); // Lock object for synchronization
    private static bool _isClicking;

    /// <summary>
    ///     Entry point of the auto clicker application.
    /// </summary>
    private static void Main()
    {
        PrintLabel();

        _targetCps = GetTargetCps(); // Set the target CPS from user input or default
        Console.WriteLine("The target CPS is: {0}.", _targetCps);

        Hook.KeyPressed += OnKeyPressed; // Subscribe to key press events

        Console.WriteLine("Press any key to set the toggle key...");

        Hook.Run(); // Start hooking keyboard events
        Hook.Dispose(); // Release the hook when done
    }

    /// <summary>
    ///     Gets the target clicks per second (CPS) from the user.
    /// </summary>
    private static int GetTargetCps()
    {
        Console.Write("Enter your target CPS: ");
        if (int.TryParse(Console.ReadLine(), out var cps) && cps > 0)
            return cps;

        Console.WriteLine("Invalid input. Using default target CPS of 12.");
        return 12;
    }

    /// <summary>
    ///     Event handler for keyboard key presses.
    /// </summary>
    private static void OnKeyPressed(object? sender, KeyboardHookEventArgs args)
    {
        if (!_keyHasBeenSet)
        {
            _toggleKey = args.Data.KeyCode; // Set the toggle key for auto clicker
            _keyHasBeenSet = true;
            Console.WriteLine($"Press {_toggleKey} to toggle the auto clicker.");
        }
        else if (args.Data.KeyCode == _toggleKey)
        {
            lock (ClickingLock)
            {
                _autoClickerEnabled = !_autoClickerEnabled; // Toggle auto clicker state

                if (_autoClickerEnabled)
                {
                    Console.WriteLine("Auto clicker has been enabled.");
                    ClickStopwatch.Restart(); // Start stopwatch for measuring click intervals
                    _clickTimer = new Timer(ClickTimerCallback, null, 0, 1); // Start the timer for click simulation
                    _isClicking = true;
                }
                else
                {
                    Console.WriteLine("Auto clicker has been disabled.");
                    _clickTimer.Dispose(); // Stop the click timer
                    _isClicking = false;
                }
            }
        }
    }

    /// <summary>
    ///     Callback function for the click timer.
    /// </summary>
    private static void ClickTimerCallback(object? state)
    {
        lock (ClickingLock)
        {
            if (!_isClicking) return;
            PerformClick(); // Simulate a mouse click
            _clickTimer.Change(CalculateClickInterval(), Timeout.Infinite); // Reschedule the timer for next click
        }
    }

    /// <summary>
    ///     Calculates the interval between clicks based on the target CPS.
    /// </summary>
    private static int CalculateClickInterval()
    {
        var targetIntervalMs = 1000.0 / _targetCps; // Calculate desired interval between clicks
        var randomVariation = Random.NextDouble() * 0.2 - 0.1; // Generate a random variation up to 10%

        var adjustedIntervalMs = targetIntervalMs * (1.0 + randomVariation); // Apply random variation
        return (int)Math.Max(adjustedIntervalMs, 1); // Ensure the interval is at least 1ms
    }

    /// <summary>
    ///     Simulates a mouse click action.
    /// </summary>
    private static void PerformClick()
    {
        double clickDurationMs = Random.Next(8, 12); // Simulate variable click duration
        Simulator.SimulateMousePress(MouseButton.Button1); // Simulate mouse button press
        Thread.Sleep((int)clickDurationMs); // Simulate click duration
        PerformRelease(); // Release the mouse button after click
    }

    /// <summary>
    ///     Simulates releasing the mouse button after a click.
    /// </summary>
    private static void PerformRelease()
    {
        Simulator.SimulateMouseRelease(MouseButton.Button1); // Simulate mouse button release

        // Simulate a short pause between clicks
        var interClickPauseMs = Random.Next(8, 20);
        Thread.Sleep(interClickPauseMs);
    }

    /// <summary>
    ///     Prints the ASCII art label for the auto clicker.
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