using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{
    public class ProgramSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }
        
        public string SettingKey { get; set; }

        public int SettingNumber { get; set; }

        public int SettingValue { get; set; }
    }
}
