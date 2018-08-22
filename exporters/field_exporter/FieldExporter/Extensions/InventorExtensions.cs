using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace FieldExporter.Extensions
{
    public static class InventorExtensions
    {
        public static string GetFullPath(this Inventor.ComponentOccurrence occ)
        {
            if (occ == null)
                return "";

            if (occ.ParentOccurrence == null)
                return occ.Name;
            else
                return occ.ParentOccurrence.GetFullPath() + "\\" + occ.Name;
        }
    }
}
