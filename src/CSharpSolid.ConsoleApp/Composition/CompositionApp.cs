// Composition Principle Demo
// Run this from the ConsoleApp to see composition in action

using System;
using CSharpSolid.Oop.Composition;

namespace CSharpSolid.ConsoleApp
{
    // Demo program
    public class CompositionBasicDemo
    {
        public static void Run()
        {
            Console.WriteLine("🎯 COMPOSITION PRINCIPLE DEMO");
            Console.WriteLine("=============================\n");

            Console.WriteLine("📋 What is Composition?");
            Console.WriteLine("Composition is building complex objects by combining simpler objects.");
            Console.WriteLine("It's a 'HAS-A' relationship instead of 'IS-A' inheritance.\n");

            // Create individual components
            Console.WriteLine("🔧 Creating components:");
            var v8Engine = new Engine("V8 Gasoline", 450);
            var sportWheels = new Wheels(4, "Sport Alloy");

            Console.WriteLine($"- Engine: {v8Engine.Type}, {v8Engine.Horsepower} HP");
            Console.WriteLine($"- Wheels: {sportWheels.Count} {sportWheels.Type}\n");

            // Compose them into a Car
            Console.WriteLine("🚗 Composing Car from components:");
            var sportsCar = new Car("Mustang GT", "Red", v8Engine, sportWheels);
            Console.WriteLine($"- Created: {sportsCar.Color} {sportsCar.Model}");
            Console.WriteLine($"- Engine Info: {sportsCar.GetEngineInfo()}\n");

            // Use the composed object
            Console.WriteLine("🎮 Using the composed Car:");
            sportsCar.Start();
            sportsCar.Drive();
            sportsCar.Stop();

            Console.WriteLine("\n✅ Key Benefits of Composition:");
            Console.WriteLine("1. 🔄 Flexibility - Mix and match different components");
            Console.WriteLine("2. ♻️  Reusability - Components work in different contexts");
            Console.WriteLine("3. 🧪 Testability - Test components independently");
            Console.WriteLine("4. 🛠️  Maintainability - Change one component without affecting others");
            Console.WriteLine("5. ⚡ Runtime Changes - Swap components dynamically");

            Console.WriteLine("\n📊 Compare with Inheritance:");
            Console.WriteLine("❌ Inheritance: Car IS-A Vehicle (rigid, hard to change)");
            Console.WriteLine("✅ Composition: Car HAS-A Engine, HAS-A Wheels (flexible, modular)");

            // Demonstrate flexibility - same components, different car
            Console.WriteLine("\n🔄 Demonstrating Flexibility:");
            var luxuryWheels = new Wheels(4, "Chrome");
            var luxuryCar = new Car("Mercedes S-Class", "Black", v8Engine, luxuryWheels);
            Console.WriteLine($"- Same engine, different wheels: {luxuryCar.Color} {luxuryCar.Model}");
            luxuryCar.Start();
            luxuryCar.Stop();
        }
    }
}