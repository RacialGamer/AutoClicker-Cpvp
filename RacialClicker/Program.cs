using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

class AutoClicker
{
    static int cps = 0;
    static int delay = 1000;
    static int keybind;
    static int screen_width;
    static int screen_height;
    static int center_x;
    static int center_y;

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    const int MOUSEEVENTF_RIGHTUP = 0x0010;
    const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    const int MOUSEEVENTF_LEFTUP = 0x0004;

    static void Main()
    {
        Console.WriteLine(@"
    ____             _       ___________      __            
   / __ \____ ______(_)___ _/ / ____/ (_)____/ /_____  _____
  / /_/ / __ `/ ___/ / __ `/ / /   / / / ___/ //_/ _ \/ ___/
 / _, _/ /_/ / /__/ / /_/ / / /___/ / / /__/ ,< /  __/ /    
/_/ |_|\__,_/\___/_/\__,_/_/\____/_/_/\___/_/|_|\___/_/     
                                                            
");
        Console.Write("How much cps: ");
        cps = int.Parse(Console.ReadLine());
        if (cps == 0)
        {
            Console.Write("Invalid input info. \nrestarting the program...\n\n\n");
            Thread.Sleep(5000);
            Main();
        }
        Console.Write("Delay between clicks (in milliseconds): ");
        delay = int.Parse(Console.ReadLine());
        if (delay > 999)
        {
            Console.Write("Invalid input info, maximum amount of delay is 999. \nrestarting the program...\n\n\n");
            Thread.Sleep(5000);
            Main();
        }
        Console.Write("Keybind Code: ");
        keybind = Convert.ToInt32(Console.ReadLine(), 16);
        if (GetKeyState(keybind) == 0)
        {
            Console.Write("Invalid input info. \nCheck https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes for valid inputs. \nBy example: 0x11 \nrestarting the program...\n\n\n");
            Thread.Sleep(5000);
            Main();
        }

        screen_width = Console.WindowWidth;
        screen_height = Console.WindowHeight;
        center_x = screen_width / 2;
        center_y = screen_height / 2;

        while (true)
        {
            if ((GetKeyState(keybind) & 0x8000) != 0)
            {
                Action(center_x, center_y);
                Thread.Sleep(1000 / cps - delay);
            }
        }
    }

    static void Action()
    {
        Action(center_x, center_y);
    }

    static void Action(int x, int y)
    {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        Thread.Sleep(delay);
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }
}