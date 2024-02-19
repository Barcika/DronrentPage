using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronrentPage
{
    public class Dron
    {
        public string? Type { get; set; }
        public string? Producer { get; set; }
        public int? Range { get; set; }
        public string? PurposeUse { get; set; }
        public DateTime? TimeEntry { get; set; }

        internal static Dron FromCsv(string dbline)
        {
            string[] splits = dbline.Split('|');
            Dron dron = new Dron();
            dron.Type = splits[0];
            dron.Producer = splits[1];
            dron.Range = int.Parse(splits[2]);
            dron.PurposeUse = splits[3];
            dron.TimeEntry = DateTime.Parse(splits[4]);
            return dron;
        }

        public string ToCsv()
        {
            return $"{Type}|{Producer}|{Range}|{PurposeUse}|{TimeEntry}";
        }
        public override string ToString()
        {
            return $"Type: {Type} - Producer: {Producer} - Range: {Range} - Usable: {PurposeUse} - Entry Time: {TimeEntry}";
        }
    }
}
