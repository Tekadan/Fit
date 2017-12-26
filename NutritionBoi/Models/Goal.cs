using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.MTL.Models
{
    public class Goal
    {
        public GoalType Type { get; set; }
        public GoalIntensityType Intensity { get; set; }

        public Double AdjustBasedOnGoal(double totalBodyWeightInKg)
        {
            double adjustment = default(double);
            switch (Type)
            {
                case GoalType.Maintain:
                    adjustment = 0;
                    break;
                case GoalType.Lose:
                case GoalType.Gain:
                    adjustment = AdjustOnTotalBodyWeight(totalBodyWeightInKg);
                    break;
            }

            return adjustment;
        }

        private Double AdjustOnTotalBodyWeight(double totalBodyWeightInKg)
        {
            double adjustment = default(double);
            double adjustmentRatio = default(double);

            switch (Intensity)
            {
                case GoalIntensityType.Light:
                    adjustmentRatio = 0.01;
                    break;
                case GoalIntensityType.Moderate:
                    adjustmentRatio = 0.02;
                    break;
                case GoalIntensityType.Intense:
                    adjustmentRatio = 0.03;
                    break;
            }
            // This will take your goal for the month, and divide up into daily changes
            adjustment = (totalBodyWeightInKg * adjustmentRatio * 7700) / 28;

            if (Type == GoalType.Lose)
            {
                adjustment = adjustment * -1;
            }

            return adjustment;
        }
    }

    public enum GoalType
    {
        Unknown = 0,
        Gain = 1,
        Maintain = 2,
        Lose = 3
    }

    public enum GoalIntensityType
    {
        Maintain = 0,   // 0% of total bodyweight/month
        Light = 1,      // 1% of total bodyweight/month
        Moderate = 2,   // 2% of total bodyweight/month
        Intense = 3     // 3% of total bodyweight/month
    }
}
