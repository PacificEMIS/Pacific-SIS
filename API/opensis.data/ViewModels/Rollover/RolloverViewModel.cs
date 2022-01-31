using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.Rollover
{
    public class RolloverViewModel : CommonFields
    {
        public RolloverViewModel()
        {
            Semesters = new List<Semesters>();
        }

        public SchoolRollover? SchoolRollover { get; set; }
        public string? FullYearName { get; set; }
        public string? FullYearShortName { get; set; }
        public bool? DoesGrades { get; set; }
        public bool? DoesExam { get; set; }
        public bool? DoesComments { get; set; }
        public List<Semesters> Semesters { get; set; }
    }
}
