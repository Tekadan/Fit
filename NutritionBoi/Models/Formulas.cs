using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.MTL.Models
{
    public static class Formulas
    {
     
        // Diet Calculations
        public static double KatchMcArdle(double leanBodyMassInKG)
        {
            return 370.0 + (21.6 * leanBodyMassInKG);
        }

        public static double LeanBodyMass(double bodyWeightInKG, double bodyFatAsPercentage)
        {
            return bodyWeightInKG - (bodyWeightInKG * bodyFatAsPercentage);
        }

        public static double CalculateTDEE(double activityRate, double bmr)
        {
            return bmr * activityRate;
        }


        // Conversions
        public static double LbtoKg(double weightInLbs)
        {
            return weightInLbs * 0.453592;
        }
    }
}
