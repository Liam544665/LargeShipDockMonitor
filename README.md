# Space Engineers - Large Ship Docking Monitor
A docking monitor system for large ships in Space Engineers that displays ship name, fuel levels, and battery status across multiple displays.


Features

✅ Displays ship name on two transparent LCDs (centered)
✅ Shows fuel (hydrogen) and power (battery) stats with progress bars
✅ Button panel display with detailed statistics
✅ Multiple port support with customizable identifiers
✅ Auto-detection of docked ships
✅ Real-time monitoring with debug output

Installation
1. Block Setup
Name your blocks with the port identifier (e.g., L - Port1):
Connector [L - Port1]
Ship Name LCD 1 [L - Port1]
Ship Name LCD 2 [L - Port1]
Ship Stats Panel [L - Port1]
2. Script Configuration
In the programmable block, change the TAG variable for each port:
csharpstring TAG = "L - Port1";  // For first large ship port
string TAG = "L - Port2";  // For second large ship port
// etc.
3. Timer Setup

Create a Timer Block
Add "Run" action for the programmable block
Set timer to trigger every 1-10 seconds
Start the timer

Usage
The script automatically monitors the connected ship and displays:

Ship Name LCDs: Centered ship name on two transparent displays
Stats Panel: Detailed fuel and power statistics with progress bars

Example Output
=== Winters Embrace ===

FUEL (H2): 87.3%
Tanks: 4
[|||||||||||||||||...]

POWER: 92.1%
Batteries: 8
45.2/49.1 MWh
[||||||||||||||||||..]
Configuration Options
You can customize these settings at the top of the script:
csharpstring TAG = "L - Port1";              // Port identifier
float FONT_SIZE_NAME = 2.0f;           // Ship name font size
float FONT_SIZE_STATS = 0.8f;          // Stats panel font size
Multiple Ports
To set up multiple large ship docking ports:

Copy the script to a new programmable block for each port
Change the TAG to L - Port2, L - Port3, etc.
Name blocks accordingly
Set up separate timers for each port

Debug Output
The programmable block displays useful debug information:
================================
  LARGE SHIP DOCK MONITOR
================================

Port Tag: [L - Port1]

Block Status:
  Connector:    [OK]
  Name LCD 1:   [OK]
  Name LCD 2:   [OK]
  Button Panel: [OK]

Connector Status: Connected
--------------------------------
DOCKED SHIP: Winters Embrace
--------------------------------

Fuel:   87.3% (4 tanks)
Power:  92.1% (8 batteries)

Displays updated successfully
Troubleshooting
No displays showing

Check block names match exactly (including brackets and spaces)
Verify the TAG matches in all block names
Check programmable block for "[MISSING]" status

Wrong information displayed

Ensure timer is running
Check connector is properly connected
Verify ship has hydrogen tanks/batteries on the same grid

License
MIT License - See LICENSE file for details
Author
Liam Moore (liam544665)
Created for Space Engineers
Version 1.0.0
Contributing
Feel free to submit issues or pull requests with improvements!
