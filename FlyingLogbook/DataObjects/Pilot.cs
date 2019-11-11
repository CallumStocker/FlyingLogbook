using FlyingLogbook.DataPersistence;
using FlyingLogbook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{    
    /// <summary>
    /// Representation of a single pilot
    /// </summary>
    public class Pilot
    {
        /// <summary>
        /// The database identifier of the pilot
        /// </summary>
        public int PilotId { get; set; }

        /// <summary>
        /// The name of the pilot
        /// </summary>
        public string Name { get; set; }

        public static Pilot GetOrCreatePilotFromDatabase(string name)
        {
            using (var sqlContext = new LogbookContext())
            {
                var existingPilot = sqlContext.Pilots.SingleOrDefault(p => p.Name == name);

                return existingPilot ?? new Pilot() { Name = name };
            }
        }
    }
}
