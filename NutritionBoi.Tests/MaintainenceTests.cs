using Microsoft.VisualStudio.TestTools.UnitTesting;
using NutritionBoi.MTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.Tests
{
    [TestClass]
    public class MaintainenceTests
    {
        [TestMethod]
        public void UpdateLossModerateTooMuch()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Moderate,
                    Type = GoalType.Lose
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 82.81,
                BodyFatPercentage = .1240,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Moderate,
                    Type = GoalType.Lose
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories < newMacros.Calories);
        }

        [TestMethod]
        public void UpdateLossModerateTooLittle()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Moderate,
                    Type = GoalType.Lose
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.40,
                BodyFatPercentage = .1270,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Moderate,
                    Type = GoalType.Lose
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories > newMacros.Calories);
        }

        [TestMethod]
        public void UpdateGainLightTooMuch()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Light,
                    Type = GoalType.Gain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 84.40,
                BodyFatPercentage = .1290,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Light,
                    Type = GoalType.Lose
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories > newMacros.Calories);
        }

        [TestMethod]
        public void UpdateGainLightTooLittle()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Light,
                    Type = GoalType.Gain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.70,
                BodyFatPercentage = .1290,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Light,
                    Type = GoalType.Lose
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories > newMacros.Calories);
        }

        [TestMethod]
        public void UpdateMaintainNoChange()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.AreEqual(currentMacros.Calories, newMacros.Calories);
        }

        [TestMethod]
        public void UpdateMaintainGain()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.85,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories > newMacros.Calories);
        }

        [TestMethod]
        public void UpdateMaintainLose()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.25,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);
            MacroCalculation newMacros = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.IsTrue(currentMacros.Calories < newMacros.Calories);
        }

        [TestMethod]
        public void GoalChange()
        {
            ClientInformation oldInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.55,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Maintain,
                    Type = GoalType.Maintain
                }
            };

            ClientInformation newInfo = new ClientInformation()
            {
                ActivityRate = 1.55,
                AverageWeightInKg = 83.25,
                BodyFatPercentage = .1288,
                Goal = new Goal()
                {
                    Intensity = GoalIntensityType.Moderate,
                    Type = GoalType.Gain
                }
            }; 
            
            MacroCalculation currentMacros = NutritionBoi.Logic.MTL.Calculate(oldInfo);

            MacroCalculation newMacros = NutritionBoi.Logic.MTL.Calculate(newInfo, currentMacros.CaloricMaintainanceAdjustment);
            MacroCalculation updatedMacros  = NutritionBoi.Logic.MTL.UpdateMacros(currentMacros, oldInfo, newInfo);

            // We lost more than our anticpated goal, which means it should increase our calories
            Assert.AreEqual(updatedMacros.Calories, newMacros.Calories);
        }
    }
}
