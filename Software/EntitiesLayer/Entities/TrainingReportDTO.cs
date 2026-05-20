using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesLayer.Entities
{
    public class TrainingReportDTO
    {
        public List<DateTime> WeightDates { get; set; } = new List<DateTime>();
        public List<double> WeightValues { get; set; } = new List<double>();
        public int TotalTrainings { get; set; }
        public List<Personal_record> PersonalRecords { get; set; } = new List<Personal_record>();
        public List<double> PlannedWeights { get; set; } = new List<double>();
        public List<double> ActualWeights { get; set; } = new List<double>();
        public List<string> TrainingLabels { get; set; } = new List<string>();

    }
}
