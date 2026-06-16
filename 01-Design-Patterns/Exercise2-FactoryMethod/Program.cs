using System;

namespace Exercise2_FactoryMethod
{
    abstract class Product
    {
        public abstract string Operation();
    }

    class ConcreteProductA : Product
    {
        public override string Operation() => "Result of ConcreteProductA";
    }

    class ConcreteProductB : Product
    {
        public override string Operation() => "Result of ConcreteProductB";
    }

    abstract class Creator
    {
        public abstract Product FactoryMethod();

        public string SomeOperation()
        {
            var product = FactoryMethod();
            return "Creator: The same creator's code has just worked with " + product.Operation();
        }
    }

    class ConcreteCreatorA : Creator
    {
        public override Product FactoryMethod() => new ConcreteProductA();
    }

    class ConcreteCreatorB : Creator
    {
        public override Product FactoryMethod() => new ConcreteProductB();
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("App: Launched with ConcreteCreatorA.");
            ClientCode(new ConcreteCreatorA());

            Console.WriteLine();
            Console.WriteLine("App: Launched with ConcreteCreatorB.");
            ClientCode(new ConcreteCreatorB());
        }

        static void ClientCode(Creator creator)
        {
            Console.WriteLine("Client: I'm not aware of the creator's class, but it still works.");
            Console.WriteLine(creator.SomeOperation());
        }
    }
}
