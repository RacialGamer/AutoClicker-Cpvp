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
    private static bool _randomizationCps; // Checks if you want Randomized Cps
    private static bool _toggleModeEnabled; // Toggle mode enabled or not.
    private static bool _keyIsHeld; // True, if key is held.
    private static bool _keyHasBeenSet; // Flag to indicate if the toggle key has been set.

    /// <summary>
    ///     The entry point of the application.
    /// </summary>
    private static void Main()
    {
        PrintLabel();

        _targetCps = GetTargetCps(); // Get the target CPS from user input.
        _randomizationCps = GetCpsRandomization(); // Get if CpsRandomization is false or not from user input.
        _toggleModeEnabled = GetToggle(); // Get the mode.
        if (_randomizationCps)
        {
            Console.WriteLine("The CPS range is between {0} and {1}.", _targetCps - 2, _targetCps + 2);

            _minDelay = 1000 / (_targetCps + 2); // Calculate minimum delay to achieve CPS range.
            _maxDelay = 1000 / (_targetCps - 2); // Calculate maximum delay to achieve CPS range.
        }
        else
        {
            _minDelay = 1000 / _targetCps; // Minimum delay is cps.
            _maxDelay = 1000 / _targetCps; // Maximum delay is cps.  
        }

        Hook.KeyPressed += OnKeyPressed; // Subscribe to the global key press event.
        Hook.KeyReleased += OnKeyReleased; // Subscribe to the global release event.
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
    ///     Checks if you want cps randomization upon input.
    /// </summary>
    /// <returns>true or false.</returns>
    private static bool GetCpsRandomization()
    {
        Console.Write("Do you want cps randomization? (y/n): ");
        var input = Console.ReadLine();
        if (input is "Y" or "y")
        {
            Console.WriteLine("Cps Randomization is turned On.");
            return true;
        }

        Console.WriteLine("Cps Randomization is turned Off.");
        return false;
    }

    private static bool GetToggle()
    {
        Console.Write("""
                      Please select the activation mode for your key bind:
                      1. Toggle mode (press once to activate, press again to deactivate)
                      2. Hold mode (press and hold to activate, release to deactivate)
                      Please enter the corresponding number (1/2):
                      """);

        var input = Console.ReadLine();
        if (input == "1")
        {
            Console.WriteLine("Toggle mode is turned On.");
            return true;
        }

        Console.WriteLine("Toggle mode is turned Off.");
        return false;
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
            Console.WriteLine(_toggleModeEnabled
                ? $"Press {_toggleKey} to toggle the auto clicker."
                : $"Press and hold {_toggleKey} to enable the auto clicker.");
        }
        else if (args.Data.KeyCode == _toggleKey) // Toggle the auto-clicker.
        {
            if (_toggleModeEnabled)
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
            else
            {
                _keyIsHeld = true;

                if (_autoClickerEnabled) return;

                _autoClickerEnabled = true;
                Console.WriteLine("Auto clicker has been enabled.");
                ThreadPool.QueueUserWorkItem(AutoClickLoop);
            }
        }
    }

    /// <summary>
    ///     Handles the global key release event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments containing the keyboard input data.</param>
    private static void OnKeyReleased(object? sender, KeyboardHookEventArgs args)
    {
        if (args.Data.KeyCode != _toggleKey) return;

        _keyIsHeld = false;

        if (!_autoClickerEnabled || _keyIsHeld) return;

        _autoClickerEnabled = false;
        Console.WriteLine("Auto clicker has been disabled.");
    }

    /// <summary>
    ///     Simulates the auto-click loop.
    /// </summary>
    /// <param name="state">The state object (unused).</param>
    private static void AutoClickLoop(object? state)
    {
        while (_autoClickerEnabled) PerformClick(); // Simulate a mouse click.
    }

    /// <summary>
    ///     Simulates a mouse click.
    /// </summary>
    private static void PerformClick()
    {
        var randomDelay = Random.Next(_minDelay, _maxDelay); // Generate a random delay.
        Thread.Sleep(randomDelay);
        Simulator.SimulateMousePress(MouseButton.Button2); // Simulate right mouse button press.
        Simulator.SimulateMouseRelease(MouseButton.Button2); // Simulate right mouse button release.
        Thread.Sleep(randomDelay);
        Simulator.SimulateMousePress(MouseButton.Button1); // Simulate left mouse button press.
        Simulator.SimulateMouseRelease(MouseButton.Button1); // Simulate left mouse button release.
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