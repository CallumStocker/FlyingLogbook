using FlyingLogbook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{
    /// <summary>
    /// Respresent a relationship between a trip and a pilot
    /// </summary>
    public class TripPilot
    {
        public TripPilot() { }

        public TripPilot(Trip trip, Pilot pilot, PilotPositionEnum pilotPosition)
        {
            this.Trip = trip;
            this.Pilot = pilot;
            this.PilotPosition = pilotPosition;

            this.TripId = this.Trip.TripId;
            this.PilotId = this.Pilot.PilotId;
        }

        /// <summary>
        /// The database id of this linker
        /// </summary>
        public int TripPilotId { get; set; }

        /// <summary>
        /// The database id of the trip
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// The database id of the pilot
        /// </summary>
        public int PilotId { get; set; }

        /// <summary>
        /// Object representing the trip
        /// </summary>
        public Trip Trip { get; set; }

        /// <summary>
        /// Object representing the pilot
        /// </summary>
        public Pilot Pilot { get; set; }

        /// <summary>
        /// The rank/position for the pilot on the trip
        /// </summary>
        public PilotPositionEnum PilotPosition { get; set; }
    }
}
