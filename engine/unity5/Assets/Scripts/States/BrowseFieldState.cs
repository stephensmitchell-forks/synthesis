using System;

namespace Synthesis.States
{
    public class BrowseFieldState : BrowseFileState
    {
        /// <summary>
        /// Initializes a new <see cref="BrowseFieldState"/> instance.
        /// </summary>
        public BrowseFieldState() : base("FieldDirectory", @"/home/mat/synthesis/Fields")
        {
        }
    }
}
