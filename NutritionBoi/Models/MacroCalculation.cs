using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.MTL.Models
{
    public class MacroCalculation
    {
        public double Calories { get; set; }
        public int Carbohydrates { get; set; }
        public int Fats { get; set; }
        public int Proteins { get; set; }
        public int CaloricMaintainanceAdjustment { get; set; }

        public MacroCalculation(double calories, double percentCarbs, double percentFats, double percentProteins, int caloricMaintainanceAdjustment = 0)
        {
            Carbohydrates = (int)Math.Round(calories / 4.0 * percentCarbs);
            Fats = (int)Math.Round(calories / 9.0 * percentFats);
            Proteins = (int)Math.Round(calories / 4.0 * percentProteins);
            Calories = Carbohydrates * 4 + Fats * 9 + Proteins * 4;
            CaloricMaintainanceAdjustment = caloricMaintainanceAdjustment;
        }

        public override string ToString()
        {
            int lowCarbs, highCarbs, lowFats, highFats, lowProteins, highProteins;

            lowCarbs = (int)Math.Round(Carbohydrates * .95);
            highCarbs = (int)Math.Round(Carbohydrates * 1.05);
            lowFats = (int)Math.Round(Fats * .95);
            highFats = (int)Math.Round(Fats * 1.05);
            lowProteins = (int)Math.Round(Proteins * .95);
            highProteins = (int)Math.Round(Proteins * 1.05);

            return "Client will eat:\n Calories: " + Calories + "\n Carbohyrates: " + lowCarbs + "-" + highCarbs + "g/day.\n Fats: " + lowFats + "-" + highFats + "g/day.\n Proteins: " + lowProteins + "-" + highProteins + "g/day.";  
        }
    }
}
