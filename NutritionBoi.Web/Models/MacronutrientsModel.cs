using NutritionBoi.MTL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NutritionBoi.Web.Models
{
    public class MacronutrientsModel : PCookieable<MacronutrientsModel>
    {
        [DisplayName("Total Daily Calories")]
        public int Calories { get; set; }
        [DisplayName("Caloric Adjustment")]
        public int CaloricMaintainanceAdjustment { get; set; }
        [DisplayName("Daily Carboyhydrate Range")]
        public int Carbohydrates { get; set; }
        [DisplayName("Daily Fat Range")]
        public int Fats { get; set; }
        [DisplayName("Daily Protein Range")]
        public int Proteins { get; set; }
        [DisplayName("Macros Calculated On Date")]
        public DateTime CalculatedDate { get; set; }
        [DisplayName("Recalculate Date")]
        public DateTime RecalculateDate
        {
            get
            {
                return CalculatedDate.AddDays(7);
            }
        }
        public bool NeedsUpdate
        {
            get
            {
                return (DateTime.Today - CalculatedDate).TotalDays > 7;
            }
        }

        public static string GetDisplayStringForMacronutrients(int macronutrientValue)
        {
            int low, high;

            low = (int)Math.Round(macronutrientValue * .95);
            high = (int)Math.Round(macronutrientValue * 1.05);

            return low.ToString() + "g to " + high.ToString() + "g.";
        }

        public static MacronutrientsModel Parse(MacroCalculation mtlModel)
        {
            MacronutrientsModel model = new MacronutrientsModel();
            model.CaloricMaintainanceAdjustment = mtlModel.CaloricMaintainanceAdjustment;
            model.Carbohydrates = mtlModel.Carbohydrates;
            model.Fats = mtlModel.Fats;
            model.Proteins = mtlModel.Proteins;

            model.Calories = mtlModel.CaloricMaintainanceAdjustment + mtlModel.Carbohydrates * 4 + mtlModel.Fats * 9 + mtlModel.Proteins * 4;

            model.CalculatedDate = DateTime.Today;

            return model;
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

        public MacroCalculation ToMtlModel()
        {
            double percentCarbs, percentFats, percentProteins;
            percentCarbs = Carbohydrates * 4 / Calories;
            percentFats = Fats * 9 / Calories;
            percentProteins = Proteins * 4 / Calories;


            return new MacroCalculation(Calories, percentCarbs, percentFats, percentProteins, CaloricMaintainanceAdjustment);
        }
    }
}