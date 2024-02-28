// See https://aka.ms/new-console-template for more information

AnimalFactory factory = new AnimalDogFactory();
AnimalFactory factory2 = new AnimalCatFactory();
factory.GetAnimalFactory().SayHi();
factory2.GetAnimalFactory().SayHi();

public interface IAnimalInterface
{
    void SayHi();
}

public class Cat : IAnimalInterface
{
    public void SayHi()
    {
        Console.WriteLine("我是猫");
    }
}

public class Dog : IAnimalInterface
{
    public void SayHi()
    {
        Console.WriteLine("我是狗");
    }
}



public interface AnimalFactory 
{
    IAnimalInterface GetAnimalFactory();

}


public class AnimalDogFactory: AnimalFactory
{
    public IAnimalInterface GetAnimalFactory()
    {
            return new Dog();
    }
}

public class AnimalCatFactory : AnimalFactory
{
    public IAnimalInterface GetAnimalFactory()
    {
        return new Cat();
    }
}