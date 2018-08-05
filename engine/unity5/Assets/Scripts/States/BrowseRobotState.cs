using System;

namespace Synthesis.States
{
    public class BrowseRobotState : BrowseFileState
    {
        /// <summary>
        /// Initializes a new <see cref="BrowseFileState"/> instance.
        /// </summary>
        public BrowseRobotState() : base("RobotDirectory", @"/home/mat/synthesis/Robots")
        {
        }
    }
}
