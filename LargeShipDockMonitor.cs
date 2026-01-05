/*
 * Docked Ship Monitor Script - LARGE SHIPS
 * Version: 1.0.0
 * Author: Liam Moore (liam544665)
 * 
 * Monitors connected ships via connectors and displays:
 * - Ship name on TWO transparent LCDs (centered)
 * - Name, fuel, and battery stats on button panel large display
 * 
 * SETUP INSTRUCTIONS:
 * ====================
 * 1. Change the TAG below for each LARGE ship docking port:
 *    - For Port 1, use: TAG = "L - Port1"
 *    - For Port 2, use: TAG = "L - Port2"
 *    - For Port 3, use: TAG = "L - Port3"
 *    - etc.
 * 
 * 2. Name your blocks with the matching tag:
 *    - Connector [L - Port1]
 *    - Ship Name LCD 1 [L - Port1]
 *    - Ship Name LCD 2 [L - Port1]
 *    - Ship Stats Panel [L - Port1]
 * 
 * 3. Set up a Timer Block:
 *    - Add "Run" action for this programmable block
 *    - Set timer to trigger every 1-10 seconds
 *    - Start the timer
 * 
 * 4. Repeat for each docking port with different programmable blocks
 *    and different TAG values (L - Port1, L - Port2, L - Port3, etc.)
 */

// ========== CONFIGURATION ==========
// CHANGE THIS FOR EACH PORT!
string TAG = "L - Port1"; // Options: "L - Port1", "L - Port2", "L - Port3", etc.

// Block name prefixes (usually don't need to change these)
string CONNECTOR_NAME = "Connector";
string NAME_LCD_1 = "Ship Name LCD 1";
string NAME_LCD_2 = "Ship Name LCD 2";
string BUTTON_PANEL_NAME = "Ship Stats Panel";

// Display settings
float FONT_SIZE_NAME = 2.0f;  // Font size for ship name on LCDs
float FONT_SIZE_STATS = 0.8f; // Font size for stats on button panel
// ===================================

IMyShipConnector connector;
IMyTextPanel nameLcd1;
IMyTextPanel nameLcd2;
IMyTextSurface buttonPanelScreen;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    InitializeBlocks();
}

void InitializeBlocks()
{
    connector = GridTerminalSystem.GetBlockWithName(CONNECTOR_NAME + " [" + TAG + "]") as IMyShipConnector;
    
    nameLcd1 = GridTerminalSystem.GetBlockWithName(NAME_LCD_1 + " [" + TAG + "]") as IMyTextPanel;
    if (nameLcd1 != null)
    {
        nameLcd1.ContentType = ContentType.TEXT_AND_IMAGE;
        nameLcd1.Font = "DEBUG";
        nameLcd1.FontSize = FONT_SIZE_NAME;
        nameLcd1.Alignment = TextAlignment.CENTER;
        nameLcd1.TextPadding = 0f;
    }
    
    nameLcd2 = GridTerminalSystem.GetBlockWithName(NAME_LCD_2 + " [" + TAG + "]") as IMyTextPanel;
    if (nameLcd2 != null)
    {
        nameLcd2.ContentType = ContentType.TEXT_AND_IMAGE;
        nameLcd2.Font = "DEBUG";
        nameLcd2.FontSize = FONT_SIZE_NAME;
        nameLcd2.Alignment = TextAlignment.CENTER;
        nameLcd2.TextPadding = 0f;
    }
    
    var buttonPanelBlock = GridTerminalSystem.GetBlockWithName(BUTTON_PANEL_NAME + " [" + TAG + "]") as IMyTextSurfaceProvider;
    if (buttonPanelBlock != null)
    {
        buttonPanelScreen = buttonPanelBlock.GetSurface(0);
        if (buttonPanelScreen != null)
        {
            buttonPanelScreen.ContentType = ContentType.TEXT_AND_IMAGE;
            buttonPanelScreen.Font = "Monospace";
            buttonPanelScreen.FontSize = FONT_SIZE_STATS;
            buttonPanelScreen.Alignment = TextAlignment.LEFT;
            buttonPanelScreen.TextPadding = 2f;
        }
    }
}

public void Main(string argument, UpdateType updateSource)
{
    // === DEBUG OUTPUT ===
    Echo("================================");
    Echo("  LARGE SHIP DOCK MONITOR");
    Echo("================================");
    Echo("");
    Echo("Port Tag: [" + TAG + "]");
    Echo("");
    Echo("Block Status:");
    Echo("  Connector:    " + (connector != null ? "[OK]" : "[MISSING]"));
    Echo("  Name LCD 1:   " + (nameLcd1 != null ? "[OK]" : "[MISSING]"));
    Echo("  Name LCD 2:   " + (nameLcd2 != null ? "[OK]" : "[MISSING]"));
    Echo("  Button Panel: " + (buttonPanelScreen != null ? "[OK]" : "[MISSING]"));
    Echo("");
    
    // Re-initialize if blocks are null
    if (connector == null || nameLcd1 == null || nameLcd2 == null || buttonPanelScreen == null)
    {
        Echo("WARNING: MISSING BLOCKS!");
        Echo("Searching for:");
        Echo("  " + CONNECTOR_NAME + " [" + TAG + "]");
        Echo("  " + NAME_LCD_1 + " [" + TAG + "]");
        Echo("  " + NAME_LCD_2 + " [" + TAG + "]");
        Echo("  " + BUTTON_PANEL_NAME + " [" + TAG + "]");
        Echo("");
        Echo("Re-initializing...");
        InitializeBlocks();
        return;
    }
    
    // Check connector status
    Echo("Connector Status: " + connector.Status.ToString());
    
    if (connector.Status != MyShipConnectorStatus.Connected)
    {
        Echo("Status: No ship docked");
        DisplayNoShip();
        return;
    }
    
    var otherConnector = connector.OtherConnector;
    if (otherConnector == null)
    {
        Echo("WARNING: Other connector null");
        DisplayNoShip();
        return;
    }
    
    // Get ship info
    string shipName = otherConnector.CubeGrid.CustomName;
    Echo("--------------------------------");
    Echo("DOCKED SHIP: " + shipName);
    Echo("--------------------------------");
    
    // Get hydrogen tanks
    List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(hydrogenTanks, tank => 
        tank.CubeGrid == otherConnector.CubeGrid && 
        tank.BlockDefinition.SubtypeId.Contains("Hydrogen"));
    
    double totalHydrogen = 0;
    double maxHydrogen = 0;
    foreach (var tank in hydrogenTanks)
    {
        totalHydrogen += tank.FilledRatio * tank.Capacity;
        maxHydrogen += tank.Capacity;
    }
    
    double h2Percent = maxHydrogen > 0 ? (totalHydrogen / maxHydrogen) * 100 : 0;
    
    // Get batteries
    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(batteries, battery => 
        battery.CubeGrid == otherConnector.CubeGrid);
    
    float totalStored = 0;
    float maxStored = 0;
    foreach (var battery in batteries)
    {
        totalStored += battery.CurrentStoredPower;
        maxStored += battery.MaxStoredPower;
    }
    
    float powerPercent = maxStored > 0 ? (totalStored / maxStored) * 100 : 0;
    
    // Display stats
    Echo("");
    Echo("Fuel:   " + h2Percent.ToString("F1") + "% (" + hydrogenTanks.Count + " tanks)");
    Echo("Power:  " + powerPercent.ToString("F1") + "% (" + batteries.Count + " batteries)");
    Echo("");
    Echo("Displays updated successfully");
    
    DisplayShipName(shipName);
    DisplayStats(shipName, totalHydrogen, maxHydrogen, hydrogenTanks.Count, 
                 totalStored, maxStored, batteries.Count);
}

void DisplayShipName(string shipName)
{
    if (nameLcd1 != null)
    {
        nameLcd1.WriteText(shipName);
    }
    
    if (nameLcd2 != null)
    {
        nameLcd2.WriteText(shipName);
    }
}

void DisplayStats(string shipName, double hydrogen, double maxHydrogen, int tankCount,
                  float power, float maxPower, int batteryCount)
{
    if (buttonPanelScreen == null) return;
    
    var sb = new System.Text.StringBuilder();
    
    sb.AppendLine("=== " + shipName + " ===\n");
    
    if (tankCount > 0)
    {
        double hydrogenPercent = maxHydrogen > 0 ? (hydrogen / maxHydrogen) * 100 : 0;
        sb.AppendLine("FUEL (H2): " + hydrogenPercent.ToString("F1") + "%");
        sb.AppendLine("Tanks: " + tankCount);
        sb.AppendLine(CreateBar(hydrogenPercent, 20));
        sb.AppendLine();
    }
    else
    {
        sb.AppendLine("FUEL: No H2 Tanks\n");
    }
    
    if (batteryCount > 0)
    {
        float powerPercent = maxPower > 0 ? (power / maxPower) * 100 : 0;
        sb.AppendLine("POWER: " + powerPercent.ToString("F1") + "%");
        sb.AppendLine("Batteries: " + batteryCount);
        sb.AppendLine(power.ToString("F1") + "/" + maxPower.ToString("F1") + " MWh");
        sb.AppendLine(CreateBar(powerPercent, 20));
    }
    else
    {
        sb.AppendLine("POWER: No Batteries");
    }
    
    buttonPanelScreen.WriteText(sb.ToString());
}

void DisplayNoShip()
{
    if (nameLcd1 != null)
    {
        nameLcd1.WriteText("NO SHIP\nDOCKED");
    }
    
    if (nameLcd2 != null)
    {
        nameLcd2.WriteText("NO SHIP\nDOCKED");
    }
    
    if (buttonPanelScreen != null)
    {
        buttonPanelScreen.WriteText("=== PORT " + TAG + " ===\n\nStatus: EMPTY\n\nAwaiting docking...");
    }
}

string CreateBar(double percent, int length)
{
    int filled = (int)((percent / 100.0) * length);
    filled = Math.Max(0, Math.Min(length, filled));
    
    string bar = "[";
    for (int i = 0; i < length; i++)
    {
        bar += (i < filled) ? "|" : ".";
    }
    bar += "]";
    
    return bar;
}
