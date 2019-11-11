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
    /// Representation of an airfield featured on a trip
    /// </summary>
    public class Airfield
    {
        public int AirfieldId { get; set; }

        /// <summary>
        /// The name of the airfield
        /// </summary>
        public string AirfieldName { get; set; }

        public static Airfield GetOrCreateAirfieldFromDatabase(string name)
        {
            using (var sqlContext = new LogbookContext())
            {
                var existingAirfield = sqlContext.Airfields.SingleOrDefault(a => a.AirfieldName == name);

                return existingAirfield ?? new Airfield() { AirfieldName = name };
            }
        }
    }
}
