using NutritionBoi.MTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.Logic
{
    public class MTL
    {
        private static void ProcessClient()
        {
            var clientInfo = RequestInformation();
            Calculate(clientInfo);
        }

        private static ClientInformation RequestInformation()
        {
            var inputString = string.Empty;

            while(!string.Equals(inputString, "KG") && !string.Equals(inputString, "LB"))
            {
                Console.WriteLine("Please select 'KG' or 'LB' for your ");
                inputString = Console.ReadLine();
                inputString = inputString.ToUpper().Trim();
            }

            ClientInformation clientInfo = null;

            if(string.Equals(inputString, "KG"))
            {
                clientInfo = InformationInMetric();   
            }
            else
            {
                clientInfo = InformationInImperial();
            }

            Console.WriteLine("Please input your body fat percentage (in the form 0.%%, i.e. 15% = 0.15):");
            inputString = Console.ReadLine().Trim();

            clientInfo.BodyFatPercentage = Double.Parse(inputString);

            Console.WriteLine("What is your activity level?\n 1. Sedentary\n 2.Light (1-3 Workouts/Week)\n 3.Moderate(3-5 Workouts/Week)\n 4.Heavy (5-7 Workouts/Week)\n 5.Extreme (8+ Workouts a week)\n (Please select a number)");
            inputString = Console.ReadLine().Trim();
            var activityLevel = int.Parse(inputString);

            switch (activityLevel)
            {
                case 1:
                    clientInfo.ActivityRate = 1.2;
                    break;
                case 2:
                    clientInfo.ActivityRate = 1.375;
                    break;
                case 3:
                    clientInfo.ActivityRate = 1.55;
                    break;
                case 4:
                    clientInfo.ActivityRate = 1.725;
                    break;
                case 5:
                    clientInfo.ActivityRate = 1.9;
                    break;
                default:
                    clientInfo.ActivityRate = 1.0;
                    break;
            }

            Console.WriteLine("What is your goal?\n 1. Gain Weight\n 2.Maintain Weight\n 3.Lose Weight\n (Please select a number)");
            inputString = Console.ReadLine();

            var goalNumber = int.Parse(inputString);
            Goal goal = new Goal();

            switch (goalNumber)
            {
                case 1:
                    goal.Type = GoalType.Gain;
                    break;
                case 2: 
                    goal.Type = GoalType.Maintain;
                    break;
                case 3:
                    goal.Type = GoalType.Lose;
                    break;
                default:
                    goal.Type = GoalType.Unknown;
                    break;
            }

            goal = RequestGoalInformation(goal);
            clientInfo.Goal = goal;

            return clientInfo;
        }

        private static ClientInformation InformationInMetric()
        {
            ClientInformation clientInfo = new ClientInformation();

            var inputString = string.Empty;
            Console.WriteLine("Please input your average body weight over the past week in KGs:");
            inputString = Console.ReadLine();

            clientInfo.AverageWeightInKg = Double.Parse(inputString);

            return clientInfo;
        }

        private static ClientInformation InformationInImperial()
        {
            ClientInformation clientInfo = new ClientInformation();

            var inputString = string.Empty;
            Console.WriteLine("Please input your average body weight over the past week in LBs:");
            inputString = Console.ReadLine();

            clientInfo.AverageWeightInKg = Formulas.LbtoKg(Double.Parse(inputString));

            return clientInfo;
        }

        private static Goal RequestGoalInformation(Goal goal)
        {
            string inputString = string.Empty;

            if (goal.Type == GoalType.Gain)
            {
                Console.WriteLine("What is your goal?\n 1. Light\n 2.Moderate\n 3.Intense\n (Please select a number)");
                inputString = Console.ReadLine().Trim();
            }

            if (goal.Type == GoalType.Maintain)
            {
                Console.WriteLine("There is no goal to select for maintaince!");
                goal.Intensity = GoalIntensityType.Maintain;
                return goal;
            }

            if (goal.Type == GoalType.Lose)
            {
                Console.WriteLine("What is your goal?\n 1. Light\n 2.Moderate\n 3.Intense\n (Please select a number)");
                inputString = Console.ReadLine().Trim();
            }

            var intensity = int.Parse(inputString);

            switch (intensity)
            {
                case 1:
                    goal.Intensity = GoalIntensityType.Light;
                    break;
                case 2:
                    goal.Intensity = GoalIntensityType.Moderate;
                    break;
                case 3:
                    goal.Intensity = GoalIntensityType.Intense;
                    break;
                default:
                    goal.Intensity = GoalIntensityType.Maintain;
                    break;
            }

            return goal;
        }

        public static MacroCalculation Calculate(ClientInformation clientInfo, int caloricMaintainanceAdjustment = 0)
        {
            var leanBodyMass = Formulas.LeanBodyMass(clientInfo.AverageWeightInKg, clientInfo.BodyFatPercentage);
            var basalMetabolicRate = Formulas.KatchMcArdle(leanBodyMass);
            var totalDailyEnergyExpenditure = Formulas.CalculateTDEE(clientInfo.ActivityRate, basalMetabolicRate);
            var adjustedTotalDailyEnergyExpenditure = totalDailyEnergyExpenditure + clientInfo.Goal.AdjustBasedOnGoal(clientInfo.AverageWeightInKg) + caloricMaintainanceAdjustment;

            var macroCalculation = CustomizeMacroPercentagesForGoal(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);

            Console.WriteLine(macroCalculation.ToString());

            return macroCalculation;
        }

        private static MacroCalculation CustomizeMacroPercentagesForGoal(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int caloricMaintainanceAdjustment)
        {
            MacroCalculation calculation = null;

            if (clientInfo.Goal.Type == GoalType.Maintain)
            {
                return CustomizeMacroPercentagesForMaintenance(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
            }

            if (clientInfo.Goal.Type == GoalType.Lose)
            {
                switch(clientInfo.Goal.Intensity)
                {
                    case GoalIntensityType.Light:
                        calculation = CustomizeMacroPercentagesForLossLight(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                    case GoalIntensityType.Moderate:
                        calculation = CustomizeMacroPercentagesForLossModerate(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                    case GoalIntensityType.Intense:
                        calculation = CustomizeMacroPercentagesForLossIntense(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                }

                return calculation;
            }

            if (clientInfo.Goal.Type == GoalType.Gain)
            {
                switch(clientInfo.Goal.Intensity)
                {
                    case GoalIntensityType.Light:
                        calculation = CustomizeMacroPercentagesForGainLight(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                    case GoalIntensityType.Moderate:
                        calculation = CustomizeMacroPercentagesForGainModerate(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                    case GoalIntensityType.Intense:
                        calculation = CustomizeMacroPercentagesForGainIntense(clientInfo, adjustedTotalDailyEnergyExpenditure, caloricMaintainanceAdjustment);
                        break;
                }

                return calculation;
            }

            return null;
        }

        private static MacroCalculation CustomizeMacroPercentagesForMaintenance(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .225;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossLight(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.1 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .25;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossModerate(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.2 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .2;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossIntense(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.3 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .175;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainLight(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .3;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainModerate(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 0.9 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .25;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainIntense(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure, int CaloricMaintainanceAdjustment)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 0.8 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .2;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage, CaloricMaintainanceAdjustment);
        }

        // To be used when we have already given someone their macros.
        public static MacroCalculation UpdateMacros(MacroCalculation currentMacros, ClientInformation oldClientInfo, ClientInformation updatedClientInfo)
        {
            // Goal changed, just update their macro calculation
            if (oldClientInfo.Goal.Type != updatedClientInfo.Goal.Type)
            {
                return Calculate(updatedClientInfo, currentMacros.CaloricMaintainanceAdjustment);
            }

            double changeAmount = 0;
            switch (updatedClientInfo.Goal.Intensity)
            {
                case GoalIntensityType.Maintain:
                    changeAmount = 0;
                    break;
                case GoalIntensityType.Light:
                    if (updatedClientInfo.Goal.Type == GoalType.Gain)
                    {
                        changeAmount = 0.01 / 4;
                    }
                    else
                    {
                        changeAmount = -0.01 / 4;
                    }
                    break;
                case GoalIntensityType.Moderate:
                    if (updatedClientInfo.Goal.Type == GoalType.Gain)
                    {
                        changeAmount = 0.02 / 4;
                    }
                    else
                    {
                        changeAmount = -0.02 / 4;
                    }
                    break;
                case GoalIntensityType.Intense:
                    if (updatedClientInfo.Goal.Type == GoalType.Gain)
                    {
                        changeAmount = 0.03 / 4;
                    }
                    else
                    {
                        changeAmount = -0.03 / 4;
                    }
                    break;
            }

            var goalWeightChangeInKG = oldClientInfo.AverageWeightInKg * changeAmount;
            var realWeightChangeInKG = updatedClientInfo.AverageWeightInKg - oldClientInfo.AverageWeightInKg;
            var deltaWeightChangeInKG = goalWeightChangeInKG - realWeightChangeInKG;
            var newCaloricMaintainence = 0.0;

            if (updatedClientInfo.Goal.Type == GoalType.Gain)
            {
                if (deltaWeightChangeInKG > 0.0)
                {
                    // We didn't gain enough
                    newCaloricMaintainence = RecalculateCaloriesBasedOnGoal(deltaWeightChangeInKG);
                }
                if (deltaWeightChangeInKG < 0.0)
                {
                    // We gained too much
                    newCaloricMaintainence = RecalculateCaloriesBasedOnGoal(-1 * deltaWeightChangeInKG);
                }
            }

            if (updatedClientInfo.Goal.Type == GoalType.Lose)
            {
                newCaloricMaintainence = RecalculateCaloriesBasedOnGoal(deltaWeightChangeInKG);
            }

            if (updatedClientInfo.Goal.Type == GoalType.Maintain)
            {
                newCaloricMaintainence = RecalculateCaloriesBasedOnGoal(deltaWeightChangeInKG);
            }

            var updatedCaloricMaintainence = newCaloricMaintainence + currentMacros.CaloricMaintainanceAdjustment;

            return Calculate(updatedClientInfo, (int)updatedCaloricMaintainence);
        }

        // Delta is always assumed to be calculated by 'goal - acutal'
        private static double RecalculateCaloriesBasedOnGoal(double deltaWeightChangeInKG)
        {
            return (deltaWeightChangeInKG * 7700) / 7;
        }
    }
}
