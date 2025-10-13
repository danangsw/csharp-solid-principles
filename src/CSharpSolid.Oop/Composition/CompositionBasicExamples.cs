// Basic Composition Example in C#
// This demonstrates the composition principle where a class contains objects of other classes

using System;

namespace CSharpSolid.Oop.Composition
{
    // Engine component - represents a "part" that can exist independently
    public class Engine
    {
        public string Type { get; set; }
        public int Horsepower { get; set; }

        public Engine(string type, int horsepower)
        {
            Type = type;
            Horsepower = horsepower;
        }

        public void Start()
        {
            Console.WriteLine($"🚗 Engine started: {Type} with {Horsepower} HP");
        }

        public void Stop()
        {
            Console.WriteLine("🛑 Engine stopped");
        }
    }

    // Wheels component
    public class Wheels
    {
        public int Count { get; set; }
        public string Type { get; set; }

        public Wheels(int count, string type)
        {
            Count = count;
            Type = type;
        }

        public void Rotate()
        {
            Console.WriteLine($"🔄 {Count} {Type} wheels rotating");
        }
    }

    // Car class - COMPOSED of Engine and Wheels (Has-A relationship)
    public class Car
    {
        // Composition: Car HAS-A Engine and HAS-A Wheels
        private readonly Engine _engine;
        private readonly Wheels _wheels;

        public string Model { get; set; }
        public string Color { get; set; }

        // Constructor injection - dependencies are provided from outside
        public Car(string model, string color, Engine engine, Wheels wheels)
        {
            Model = model;
            Color = color;
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _wheels = wheels ?? throw new ArgumentNullException(nameof(wheels));
        }

        // Car delegates work to its composed parts
        public void Start()
        {
            Console.WriteLine($"🚦 Starting {Color} {Model}...");
            _engine.Start();
        }

        public void Drive()
        {
            Console.WriteLine($"🏎️  {Model} is driving...");
            _wheels.Rotate();
        }

        public void Stop()
        {
            Console.WriteLine($"⏹️  Stopping {Model}...");
            _engine.Stop();
        }

        // Can access information from components
        public string GetEngineDetails() => $"Engine Type: {_engine.Type}, Horsepower: {_engine.Horsepower}";
        public string GetWheelsDetails() => $"Wheels Model: {_wheels.Count}, Type: {_wheels.Type}";
    }

    // Another example: Computer composed of CPU, Memory, Storage
    public class CPU
    {
        public string Model { get; set; }
        public int Cores { get; set; }

        public CPU(string model, int cores)
        {
            Model = model;
            Cores = cores;
        }

        public void Process()
        {
            Console.WriteLine($"CPU {Model} processing with {Cores} cores");
        }
    }

    public class Memory
    {
        public int SizeGB { get; set; }

        public Memory(int sizeGB)
        {
            SizeGB = sizeGB;
        }

        public void Allocate()
        {
            Console.WriteLine($"Allocating {SizeGB}GB memory");
        }
    }

    public class Storage
    {
        public int SizeGB { get; set; }
        public string Type { get; set; }

        public Storage(int sizeGB, string type)
        {
            SizeGB = sizeGB;
            Type = type;
        }

        public void Read()
        {
            Console.WriteLine($"Reading from {SizeGB}GB {Type} storage");
        }
    }

    public class Computer
    {
        // Composition: Computer HAS-A CPU, HAS-A Memory, HAS-A Storage
        private CPU _cpu;
        private Memory _memory;
        private Storage _storage;

        public string Brand { get; set; }

        public Computer(string brand, CPU cpu, Memory memory, Storage storage)
        {
            Brand = brand;
            _cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void Boot()
        {
            Console.WriteLine($"Booting {Brand} computer...");
            _memory.Allocate();
            _cpu.Process();
            Console.WriteLine("Computer booted successfully!");
        }

        public void RunProgram()
        {
            Console.WriteLine("Running program...");
            _cpu.Process();
            _memory.Allocate();
            _storage.Read();
        }
    }
}   