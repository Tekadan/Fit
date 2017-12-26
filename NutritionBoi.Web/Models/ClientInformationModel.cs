using NutritionBoi.MTL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NutritionBoi.Web.Models
{
    public class ClientInformationModel : PCookieable<ClientInformationModel>
    {
        [DisplayName("Average Body Weight Over Past Week")]
        public double AverageWeightInKg { get; set; }

        [DisplayName("Body Fat Percentage")]
        public double BodyFatPercentage { get; set; }

        [DisplayName("Activity Rate")]
        public double ActivityRate { get; set; }

        [DisplayName("Diet Goal")]
        public string GoalType { get; set; }

        [DisplayName("Intensity of Diet Goal")]
        public string GoalIntensity { get; set; }

        public ClientInformation ToMtlModel()
        {
            ClientInformation mtlModel = new ClientInformation()
            {
                ActivityRate = ActivityRate,
                AverageWeightInKg = AverageWeightInKg,
                BodyFatPercentage = BodyFatPercentage,
                Goal = new Goal()
            };

            switch (GoalType)
            {
                case "Lose":
                    mtlModel.Goal.Type = MTL.Models.GoalType.Lose;
                    break;
                case "Maintain":
                    mtlModel.Goal.Type = MTL.Models.GoalType.Maintain;
                    break;
                case "Gain":
                    mtlModel.Goal.Type = MTL.Models.GoalType.Gain;
                    break;
            }

            switch (GoalIntensity)
            {
                case "Light":
                    mtlModel.Goal.Intensity = MTL.Models.GoalIntensityType.Light;
                    break;
                case "Moderate":
                    mtlModel.Goal.Intensity = MTL.Models.GoalIntensityType.Moderate;
                    break;
                case "Intense":
                    mtlModel.Goal.Intensity = MTL.Models.GoalIntensityType.Intense;
                    break;
            }

            return mtlModel;
        }
    }
}