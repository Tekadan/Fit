using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.MTL.Models
{
    public class ClientInformation
    {
        public double AverageWeightInKg { get; set; }
        public double BodyFatPercentage { get; set; }
        public double ActivityRate { get; set; }
        public Goal Goal { get; set; }

        public ClientInformation()
        {

        }
    }
}
