using NutritionBoi.MTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionBoi.MTL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProcessClient();
            Console.WriteLine("Press and key to finish and close.");
            Console.ReadKey();
        }

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

        private static void Calculate(ClientInformation clientInfo)
        {
            var leanBodyMass = Formulas.LeanBodyMass(clientInfo.AverageWeightInKg, clientInfo.BodyFatPercentage);
            var basalMetabolicRate = Formulas.KatchMcArdle(leanBodyMass);
            var totalDailyEnergyExpenditure = Formulas.CalculateTDEE(clientInfo.ActivityRate, basalMetabolicRate);
            var adjustedTotalDailyEnergyExpenditure = totalDailyEnergyExpenditure + clientInfo.Goal.AdjustBasedOnGoal(clientInfo.AverageWeightInKg);

            var macroCalculation = CustomizeMacroPercentagesForGoal(clientInfo, adjustedTotalDailyEnergyExpenditure);

            Console.WriteLine(macroCalculation.ToString());
        }

        private static MacroCalculation CustomizeMacroPercentagesForGoal(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            MacroCalculation calculation = null;

            if (clientInfo.Goal.Type == GoalType.Maintain)
            {
                return CustomizeMacroPercentagesForMaintenance(clientInfo, adjustedTotalDailyEnergyExpenditure);
            }

            if (clientInfo.Goal.Type == GoalType.Lose)
            {
                switch(clientInfo.Goal.Intensity)
                {
                    case GoalIntensityType.Light:
                        calculation = CustomizeMacroPercentagesForLossLight(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                    case GoalIntensityType.Moderate:
                        calculation = CustomizeMacroPercentagesForLossModerate(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                    case GoalIntensityType.Intense:
                        calculation = CustomizeMacroPercentagesForLossIntense(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                }

                return calculation;
            }

            if (clientInfo.Goal.Type == GoalType.Gain)
            {
                switch(clientInfo.Goal.Intensity)
                {
                    case GoalIntensityType.Light:
                        calculation = CustomizeMacroPercentagesForGainLight(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                    case GoalIntensityType.Moderate:
                        calculation = CustomizeMacroPercentagesForGainModerate(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                    case GoalIntensityType.Intense:
                        calculation = CustomizeMacroPercentagesForGainIntense(clientInfo, adjustedTotalDailyEnergyExpenditure);
                        break;
                }

                return calculation;
            }

            return null;
        }

        private static MacroCalculation CustomizeMacroPercentagesForMaintenance(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .225;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossLight(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.1 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .25;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossModerate(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.2 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .2;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForLossIntense(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 1.3 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .175;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainLight(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .3;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainModerate(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 0.9 * 2.2 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .25;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }

        private static MacroCalculation CustomizeMacroPercentagesForGainIntense(ClientInformation clientInfo, double adjustedTotalDailyEnergyExpenditure)
        {
            // 2.2/g protein per kg of bodyweight
            var proteinPercentage = (clientInfo.AverageWeightInKg * 0.8 * 4) / adjustedTotalDailyEnergyExpenditure;
            var fatPercentage = .2;
            var carbohydratePercentage = 1.00 - fatPercentage - proteinPercentage;

            return new MacroCalculation(adjustedTotalDailyEnergyExpenditure, carbohydratePercentage, fatPercentage, proteinPercentage);
        }
    }
}
