using FlyingLogbook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{
    /// <summary>
    /// Representation of a trip
    /// </summary>
    public class Trip
    {
        #region Constructors and Setup

        public Trip()
        {
            this.Date = DateTime.Today;
            this.Pilots = new List<TripPilot>();
            this.Airfields = new List<TripAirfield>();
        }

        public Trip(
            int tripId, 
            DateTime date, 
            string planeType, 
            string regNumber, 
            List<Tuple<Pilot, PilotPositionEnum>> pilots, 
            List<Tuple<Airfield, AirfieldDirectionEnum>> airfields,
            DateTime departureTime,
            DateTime arrivalTime, 
            TimeSpan dayHours, 
            TimeSpan nightHours, 
            double bTRT, 
            bool landed, 
            string remarks)
        {
            this.TripId = tripId;
            this.Date = date;
            this.PlaneType = planeType;
            this.RegNumber = regNumber;
            this.Pilots = pilots.Select(p => new TripPilot(this, p.Item1, p.Item2)).ToList();
            this.Airfields = airfields.Select(a => new TripAirfield(this, a.Item1, a.Item2)).ToList();
            this.DepartureTime = departureTime;
            this.ArrivalTime = arrivalTime;
            this.DayHours = dayHours;
            this.NightHours = nightHours;
            this.BTRT = bTRT;
            this.Landed = landed;
            this.Remarks = remarks;
        }

        #endregion

        #region Public Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TripId { get; set; }

        /// <summary>
        /// The date the trip started on - time component is ignored
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// The model number of the plane for the trip
        /// </summary>
        [MaxLength(10)]
        public string PlaneType { get; set; }

        /// <summary>
        /// The registration number of the plane for trip
        /// </summary>
        [MaxLength(10)]
        public string RegNumber { get; set; }

        /// <summary>
        /// List of pilots on the trip
        /// </summary>
        public List<TripPilot> Pilots { get; set; }

        /// <summary>
        /// List of airfield used on the trip
        /// </summary>
        public List<TripAirfield> Airfields { get; set; }

        /// <summary>
        /// The time the trip departed - date component is ignored
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// The time the trip arrived - date component is ignored
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// The number of daytime hours flown on the trip
        /// </summary>
        [NotMapped]
        public TimeSpan DayHours { get; set; }

        /// <summary>
        /// Wrapper for the DayHours property so it appears as a double - the EF SQLite stuff doesn't like TimeSpans
        /// </summary>
        public double DayHoursDouble
        {
            get
            {
                // Dividing the minutes by 60 to turn them into a fraction of 100
                return this.DayHours.Hours + (this.DayHours.Minutes / 60.0);
            }

            set
            {
                var hours = (int)value;
                var minutes = (int)((value - hours) * 60);

                this.DayHours = new TimeSpan(hours, minutes, 0);
            }
        }

        /// <summary>
        /// The number of nightime hours flown on the trip
        /// </summary>
        [NotMapped]
        public TimeSpan NightHours { get; set; }

        /// <summary>
        /// Wrapper for the NightHours property so it appears as a double - the EF SQLite stuff doesn't like TimeSpans
        /// </summary>
        public double NightHoursDouble
        {
            get
            {
                // Dividing the minutes by 60 to turn them into a fraction of 100
                return this.NightHours.Hours + (this.NightHours.Minutes / 60.0);
            }

            set
            {
                var hours = (int)value;
                var minutes = (int)((value - hours) * 60);

                this.NightHours = new TimeSpan(hours, minutes, 0);
            }
        }

        /// <summary>
        /// The number of hours this trip is expected to take - tracked as hours and fractions of hours, so double
        /// </summary>
        public double BTRT { get; set; }

        /// <summary>
        /// If the logbook pilot landed the trip
        /// </summary>
        public bool Landed { get; set; }
        
        /// <summary>
        /// Any recorded remarks for the flight
        /// </summary>
        [MaxLength(1000)]
        public string Remarks { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var outputBuilder = new StringBuilder();

            outputBuilder
                .AppendLine($"Date: {this.Date}")
                .AppendLine($"Plane Type: {this.PlaneType}")
                .AppendLine($"Reg Number: {this.RegNumber}")
                .AppendLine($"Pilots: ");

            foreach (var pilotLinker in this.Pilots)
            {
                outputBuilder.AppendLine(
                    $"\tName: {pilotLinker.Pilot.Name} Type: {Enum.GetName(typeof(PilotPositionEnum), pilotLinker.PilotPosition)}");
            }

            outputBuilder.AppendLine($"Airfields: ");

            foreach (var airfieldLinker in this.Airfields)
            {
                outputBuilder.AppendLine(
                    $"\tName: {airfieldLinker.Airfield.AirfieldName} Direction: {Enum.GetName(typeof(AirfieldDirectionEnum), airfieldLinker.AirfieldDirection)}");
            }

            outputBuilder
                .AppendLine($"Departure Time: {this.DepartureTime}")
                .AppendLine($"Arrival Time: {this.ArrivalTime}")
                .AppendLine($"Day Hours: {this.DayHours}")
                .AppendLine($"Night Hours: {this.NightHours}")
                .AppendLine($"BTRT: {this.BTRT}")
                .AppendLine($"Landed: {this.Landed}")
                .AppendLine($"Remarks: {this.Remarks}");            

            return outputBuilder.ToString();
        }

        #endregion

        #region Static Methods

        public static Trip LoadTripFromLegacy(string tripLine, int tripId, List<Pilot> pilots, List<Airfield> airfields)
        {            
            var sections = tripLine.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            var timeSplitChars = new char[] { '.', ',' };

            if (sections.Length < 14)
            {
                throw new Exception("Insufficient items on row");
            }

            DateTime date;
            string planeType, regNumber, remarks;
            var pilotLinkers = new List<Tuple<Pilot, PilotPositionEnum>>();
            var airfieldLinkers = new List<Tuple<Airfield, AirfieldDirectionEnum>>();
            DateTime departureTime, arrivalTime;
            TimeSpan dayHours, nightHours;
            double btrt;
            bool landed;

            if (!DateTime.TryParse(sections[0], out date))
            {
                throw new Exception($"{sections[0]} is not recognised as a date");
            }

            planeType = sections[1];
            regNumber = sections[2];

            int lastPilotIndex = 3;

            while (!IsAirfield(sections[lastPilotIndex]))
            {
                var pilotName = sections[lastPilotIndex];

                var foundPilot = pilots.SingleOrDefault(p => p.Name == pilotName);

                if (foundPilot == null)
                {
                    foundPilot = new Pilot() { Name = pilotName };
                    pilots.Add(foundPilot);
                }

                pilotLinkers.Add(
                    new Tuple<Pilot, PilotPositionEnum>(
                        foundPilot, 
                        lastPilotIndex == 3 ? PilotPositionEnum.Captain : PilotPositionEnum.FirstOfficer
                    )
                );

                lastPilotIndex++;
            }

            var departureAirfieldName = sections[lastPilotIndex];

            var departureAirfield = airfields.SingleOrDefault(a => a.AirfieldName == departureAirfieldName);

            if (departureAirfield == null)
            {
                departureAirfield = new Airfield() { AirfieldName = departureAirfieldName };
                airfields.Add(departureAirfield);
            }

            airfieldLinkers.Add(
                new Tuple<Airfield, AirfieldDirectionEnum>(
                    departureAirfield,
                    AirfieldDirectionEnum.Departure)
            );

            var arrivalAirfieldName = sections[lastPilotIndex + 1];

            var arrivalAirfield = airfields.SingleOrDefault(a => a.AirfieldName == arrivalAirfieldName);

            if (arrivalAirfield == null)
            {
                arrivalAirfield = new Airfield() { AirfieldName = arrivalAirfieldName };
                airfields.Add(arrivalAirfield);
            }           

            airfieldLinkers.Add(
                new Tuple<Airfield, AirfieldDirectionEnum>(
                    arrivalAirfield,
                    AirfieldDirectionEnum.Arrival)
            );

            var departureformatString = sections[lastPilotIndex + 2].Contains('.')
                ? "HH.mm"
                : "HH,mm";

            if (DateTime.TryParseExact(
                sections[lastPilotIndex + 2],
                departureformatString, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out DateTime departureDateTime))
            {
                departureTime = departureDateTime;
            }
            else
            {
                throw new Exception($"{sections[lastPilotIndex + 2]} is not recognised as a time");
            }

            var arrivalformatString = sections[lastPilotIndex + 3].Contains('.')
                ? "HH.mm"
                : "HH,mm";

            if (DateTime.TryParseExact(
                sections[lastPilotIndex + 3],
                arrivalformatString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime arrivalDateTime))
            {
                arrivalTime = arrivalDateTime;
            }
            else
            {
                throw new Exception($"{sections[lastPilotIndex + 3]} is not recognised as a time");
            }

            try
            {
                var dayHoursParts = sections[lastPilotIndex + 4].Split(timeSplitChars);

                var minutesString = dayHoursParts[1].Length == 1
                    ? dayHoursParts[1] + "0"
                    : dayHoursParts[1];

                dayHours = new TimeSpan(
                    int.Parse(dayHoursParts[0]), 
                    int.Parse(minutesString), 
                    0);
            }
            catch (Exception)
            {
                throw new Exception($"{sections[lastPilotIndex + 4]} is not recognised as a time");
            }

            try
            {
                var nightHoursParts = sections[lastPilotIndex + 5].Split(timeSplitChars);

                var minutesString = nightHoursParts[1].Length == 1
                    ? nightHoursParts[1] + "0"
                    : nightHoursParts[1];

                nightHours = new TimeSpan(
                    int.Parse(nightHoursParts[0]),
                    int.Parse(minutesString),
                    0);
            }
            catch (Exception)
            {
                throw new Exception($"{sections[lastPilotIndex + 5]} is not recognised as a time");
            }

            if (!double.TryParse(sections[lastPilotIndex + 6], out btrt))
            {
                throw new Exception($"{sections[lastPilotIndex + 6]} is not recognised as a double");
            }

            landed = sections[lastPilotIndex + 7] == "y";
            remarks = sections[lastPilotIndex + 8];

            var newTrip = new Trip(            
                tripId,
                date,
                planeType,
                regNumber,
                pilotLinkers,
                airfieldLinkers,
                departureTime,
                arrivalTime,
                dayHours,
                nightHours,
                btrt,
                landed,
                remarks
            );

            Debug.WriteLine(newTrip);

            return newTrip;
        }

        private static bool IsAirfield(string possibleAirfield)
        {
            return possibleAirfield.Length == 3 && possibleAirfield.All(c => Char.IsUpper(c));
        }

        #endregion
    }
}
