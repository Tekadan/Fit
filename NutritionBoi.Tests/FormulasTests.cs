using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NutritionBoi.MTL.Models;

namespace NutritionBoi.Tests
{
    [TestClass]
    public class FormulasTests
    {
        [TestMethod]
        public void CalculateLossLight()
        {
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ActivityRate = 1.55;
            clientInfo.AverageWeightInKg = Formulas.LbtoKg(184);
            clientInfo.BodyFatPercentage = .1288;
            clientInfo.Goal = new Goal
            {
                Intensity = GoalIntensityType.Light,
                Type = GoalType.Lose
            };

            Assert.AreEqual(Math.Round(clientInfo.AverageWeightInKg), 83);

            var leanBodyMass = Formulas.LeanBodyMass(clientInfo.AverageWeightInKg, clientInfo.BodyFatPercentage);
            Assert.AreEqual(Math.Round(leanBodyMass), 73);
            
            var basalMetabolicRate = Formulas.KatchMcArdle(leanBodyMass);
            Assert.AreEqual(Math.Round(basalMetabolicRate), 1941);

            var totalDailyEnergyExpenditure = Formulas.CalculateTDEE(clientInfo.ActivityRate, basalMetabolicRate);
            Assert.AreEqual(Math.Round(totalDailyEnergyExpenditure), 3008);

            var adjustedTotalDailyEnergyExpenditure = totalDailyEnergyExpenditure + clientInfo.Goal.AdjustBasedOnGoal(clientInfo.AverageWeightInKg);
            Assert.AreEqual(Math.Round(adjustedTotalDailyEnergyExpenditure), 2778);

        }

        [TestMethod]
        public void CalculateMaintain()
        {
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ActivityRate = 1.2;
            clientInfo.AverageWeightInKg = Formulas.LbtoKg(104);
            clientInfo.BodyFatPercentage = .18;
            clientInfo.Goal = new Goal
            {
                Intensity = GoalIntensityType.Maintain,
                Type = GoalType.Maintain
            };

            Assert.AreEqual(Math.Round(clientInfo.AverageWeightInKg), 47);

            var leanBodyMass = Formulas.LeanBodyMass(clientInfo.AverageWeightInKg, clientInfo.BodyFatPercentage);
            Assert.AreEqual(Math.Round(leanBodyMass), 39);
            
            var basalMetabolicRate = Formulas.KatchMcArdle(leanBodyMass);
            Assert.AreEqual(Math.Round(basalMetabolicRate), 1206);

            var totalDailyEnergyExpenditure = Formulas.CalculateTDEE(clientInfo.ActivityRate, basalMetabolicRate);
            Assert.AreEqual(Math.Round(totalDailyEnergyExpenditure), 1447);

            var adjustedTotalDailyEnergyExpenditure = totalDailyEnergyExpenditure + clientInfo.Goal.AdjustBasedOnGoal(clientInfo.AverageWeightInKg);
            Assert.AreEqual(Math.Round(adjustedTotalDailyEnergyExpenditure), 1447);
        }

        [TestMethod]
        public void CalculateGainModerate()
        {
            ClientInformation clientInfo = new ClientInformation();
            clientInfo.ActivityRate = 1.375;
            clientInfo.AverageWeightInKg = Formulas.LbtoKg(198);
            clientInfo.BodyFatPercentage = 0.15;
            clientInfo.Goal = new Goal
            {
                Intensity = GoalIntensityType.Moderate,
                Type = GoalType.Gain
            };

            Assert.AreEqual(Math.Round(clientInfo.AverageWeightInKg), 90);

            var leanBodyMass = Formulas.LeanBodyMass(clientInfo.AverageWeightInKg, clientInfo.BodyFatPercentage);
            var basalMetabolicRate = Formulas.KatchMcArdle(leanBodyMass);
            var totalDailyEnergyExpenditure = Formulas.CalculateTDEE(clientInfo.ActivityRate, basalMetabolicRate);
            var adjustedTotalDailyEnergyExpenditure = totalDailyEnergyExpenditure + clientInfo.Goal.AdjustBasedOnGoal(clientInfo.AverageWeightInKg);
        }
    }
}
