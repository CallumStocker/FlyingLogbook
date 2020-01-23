using FlyingLogbook.DataObjects;
using FlyingLogbook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.UIDataObjects
{
    /// <summary>
    /// A wrapper for a Trip with extra properties to allow easier data binding
    /// </summary>
    public class UITrip
    {
        #region Constructors and Setup

        public UITrip(Trip dataObject)
        {
            this.DataObject = dataObject;
        }

        #endregion

        #region Properties

        public Trip DataObject { get; set; }

        public DateTime Date
        { 
            get
            {
                return this.DataObject.Date;
            }
        }

        public string PlaneType
        {
            get
            {
                return this.DataObject.PlaneType;
            }
        }

        public string RegNumber
        {
            get
            {
                return this.DataObject.RegNumber;
            }
        }

        public string Captain
        {
            get
            {
                return this.DataObject.Pilots.FirstOrDefault(p => p.PilotPosition == PilotPositionEnum.Captain)?.Pilot?.Name;
            }
        }

        public string FirstOfficerOne
        {
            get
            {
                return this.DataObject.Pilots
                    .Where(p => p.PilotPosition == PilotPositionEnum.FirstOfficer)
                    .FirstOrDefault()?.Pilot?.Name;
            }
        }

        public string FirstOfficerTwo
        {
            get
            {
                return this.DataObject.Pilots
                    .Where(p => p.PilotPosition == PilotPositionEnum.FirstOfficer)
                    .Skip(1)
                    .FirstOrDefault()?.Pilot?.Name;
            }
        }

        public string FirstOfficerThree
        {
            get
            {
                return this.DataObject.Pilots
                    .Where(p => p.PilotPosition == PilotPositionEnum.FirstOfficer)
                    .Skip(2)
                    .FirstOrDefault()?.Pilot?.Name;
            }
        }

        public string DepartureAirfield
        {
            get
            {
                return this.DataObject.Airfields.FirstOrDefault(a => a.AirfieldDirection == AirfieldDirectionEnum.Departure)?.Airfield?.AirfieldName;
            }
        }

        public string ArrivalAirfield
        {
            get
            {
                return this.DataObject.Airfields.FirstOrDefault(a => a.AirfieldDirection == AirfieldDirectionEnum.Arrival)?.Airfield?.AirfieldName;
            }
        }

        public DateTime DepartureTime
        {
            get
            {
                return this.DataObject.DepartureTime;
            }
        }

        public DateTime ArrivalTime
        {
            get
            {
                return this.DataObject.ArrivalTime;
            }
        }

        public TimeSpan DayHours
        {
            get
            {
                return this.DataObject.DayHours;
            }
        }

        public TimeSpan NightHours
        {
            get
            {
                return this.DataObject.NightHours;
            }
        }

        public double BTRT
        {
            get
            {
                return this.DataObject.BTRT;
            }
        }

        public bool Landed
        {
            get
            {
                return this.DataObject.Landed;
            }
        }

        public string Remarks
        {
            get
            {
                return this.DataObject.Remarks;
            }
        }

        #endregion
    }
}
