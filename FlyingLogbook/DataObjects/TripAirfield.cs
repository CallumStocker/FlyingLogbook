using FlyingLogbook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{
    /// <summary>
    /// Respresent a relationship between a trip and an airfield
    /// </summary>
    public class TripAirfield
    {
        public TripAirfield() { }

        public TripAirfield(Trip trip, Airfield airfield, AirfieldDirectionEnum airfieldDirection)
        {
            this.Trip = trip;
            this.Airfield = airfield;
            this.AirfieldDirection = airfieldDirection;

            this.TripId = this.Trip.TripId;
            this.AirfieldId = this.Airfield.AirfieldId;
        }

        /// <summary>
        /// The database id of this linker
        /// </summary>
        public int TripAirfieldId { get; set; }

        /// <summary>
        /// The database id of the trip
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// The database id of the airfield
        /// </summary>
        public int AirfieldId { get; set; }

        /// <summary>
        /// Object representing the trip
        /// </summary>
        public Trip Trip { get; set; }

        /// <summary>
        /// Object representing the airfield
        /// </summary>
        public Airfield Airfield { get; set; }

        /// <summary>
        /// Whether the trip departed or arrived at the airfield
        /// </summary>
        public AirfieldDirectionEnum AirfieldDirection { get; set; }
    }
}
