using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
using Nautilus.Commands;

namespace MoreLeviathanSpawns
{
    public static class LeviathanCommands
    {
        //Warps the player to the desired leviathan
        [ConsoleCommand("coordwarp")]
        public static void CoordWarp(int index)
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            //If not even reapercoords was populated, neither will ghost, so this works as a test
            if (ModEnabled())
            {
                //Set total count of both types of leviathans for reference
                int reaperCount = saveCoords.ReaperCoords.Count;
                int ghostCount = saveCoords.GhostCoords.Count;

                //Attempt to warp to reaper or ghost leviathan, if attempted index is within the range of either of the totals of both leivathans
                //'1' to 'total number of reapers' = reaper warp, 'total numbers of reapers' + 1 to 'total number of reapers' + 'total number of ghosts' = ghost warp
                if (index >= 1 && index <= (reaperCount + ghostCount))
                {
                    if (index <= reaperCount)
                    {
                        ErrorMessage.AddMessage($"Teleporting to Reaper spawn #{index} - Coords: {saveCoords.ReaperCoords[index - 1].Coord}");
                        logger.LogInfo($"Teleporting to Reaper spawn #{index} - Coords: {saveCoords.ReaperCoords[index - 1].Coord}");
                        Player.main.SetPosition(saveCoords.ReaperCoords[index - 1].Coord);
                    }
                    else
                    {
                        ErrorMessage.AddMessage($"Teleporting to Ghost spawn #{index - reaperCount} - Coords: {saveCoords.GhostCoords[index - reaperCount - 1].Coord}");
                        logger.LogInfo($"Teleporting to Ghost spawn #{index - reaperCount} - Coords: {saveCoords.GhostCoords[index - reaperCount - 1].Coord}");
                        Player.main.SetPosition(saveCoords.GhostCoords[index - reaperCount - 1].Coord);
                    }
                }
                else
                {
                    ErrorMessage.AddError($"Index {index} out of bounds!");
                    logger.LogError($"Index {index} out of bounds!");
                }
            }
        }

        //Lists the total number of each type of leviathan spawned in the save
        [ConsoleCommand("leviathantotal")]
        public static void LeviathanTotal()
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            if (ModEnabled())
            {
                var reaperTotal = saveCoords.ReaperCoords.Count;
                var ghostTotal = saveCoords.GhostCoords.Count;
                ErrorMessage.AddMessage($"{reaperTotal + ghostTotal} leviathans spawned\n{reaperTotal} Reapers spawned and {ghostTotal} Ghosts spawned");
                logger.LogInfo($"{reaperTotal + ghostTotal} leviathans spawned; {reaperTotal} Reapers spawned and {ghostTotal} Ghosts spawned");
            }
        }

        //Lists the textual location of each leviathan spawned in the world
        [ConsoleCommand("leviathanlist")]
        public static void LeviathanList()
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            if (ModEnabled())
            {
                var reaperTotal = saveCoords.ReaperCoords.Count;
                var ghostTotal = saveCoords.GhostCoords.Count;

                //List textual locations of each reaper, if any exist
                if(reaperTotal > 0)
                {
                    for (var i = 0; i < reaperTotal; i++)
                    {
                        var reaper = saveCoords.ReaperCoords[i];

                        ErrorMessage.AddMessage($"Reaper spawn #{i+1} ({reaper.ListIndex}) located at {ReaperLocations[reaper.ListIndex]} - Coords: {reaper.Coord}");
                        logger.LogMessage($"Reaper spawn #{i + 1} ({reaper.ListIndex}) located at {ReaperLocations[reaper.ListIndex]} - Coords: {reaper.Coord}");
                    }
                }
                else
                {
                    ErrorMessage.AddMessage("No Reapers spawned to list");
                    logger.LogMessage($"No Reapers spawned to list");
                }

                //List textual locations of each ghost, if any exist
                if (ghostTotal > 0)
                {
                    for (var i = 0; i < ghostTotal; i++)
                    {
                        var ghost = saveCoords.GhostCoords[i];

                        ErrorMessage.AddMessage($"Ghost spawn #{i + 1} ({ghost.ListIndex}) located at {GhostLocations[ghost.ListIndex]} - Coords: {ghost.Coord}");
                        logger.LogMessage($"Ghost spawn #{i + 1} ({ghost.ListIndex}) located at {GhostLocations[ghost.ListIndex]} - Coords: {ghost.Coord}");
                    }
                }
                else
                {
                    ErrorMessage.AddMessage("No Ghosts spawned to list");
                    logger.LogMessage($"No Ghosts spawned to list");
                }
            }
        }
    }
}
