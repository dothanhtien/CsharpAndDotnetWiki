// C# OOP - sample project
// Run: dotnet run (from this src/ folder)

// ---- 1. Encapsulation ----
var account = new BankAccount("Alice", 100);
account.Deposit(50);
account.Withdraw(30);
Console.WriteLine($"{account.Owner}'s balance: {account.Balance}");
// account.Balance = 1000; // <- would not compile: the setter is private

try
{
    account.Withdraw(10_000);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Withdraw rejected: {ex.Message}");
}

Console.WriteLine();

// ---- 2. Inheritance & polymorphism ----
// A List<Shape> can hold any subclass; calling Area()/Describe() runs
// each object's own override - this is runtime polymorphism.
var shapes = new List<Shape>
{
    new Circle(radius: 3),
    new Rectangle(width: 4, height: 5),
};

foreach (var shape in shapes)
{
    shape.Describe();
}

Console.WriteLine();

// ---- 3. Abstraction via interfaces ----
// IMovable doesn't care what the concrete type is, only that it can Move().
var movers = new List<IMovable> { new Circle(radius: 1), new Robot("R2") };
foreach (var mover in movers)
{
    mover.Move();
}

Console.WriteLine();

// ---- 4. Static vs instance members ----
// InstanceCount is shared by the type itself, not by each object.
Console.WriteLine($"Shapes created so far: {Shape.InstanceCount}");

Console.WriteLine();

// ---- 5. Constructors & object initializers ----
// Object initializer syntax ({ Label = ... }) sets an init-only property
// right after the constructor runs.
var square = new Rectangle(width: 2, height: 2) { Label = "Square-ish rectangle" };
Console.WriteLine($"{square.Label}: area = {square.Area()}");

Console.WriteLine();

// ---- 6. Method overloading ----
// Same method name, different parameter lists - resolved at compile time.
account.Describe();
account.Describe(prefix: "Statement:");

Console.WriteLine();

// ---- 7. Overriding vs hiding (override vs new) ----
// override participates in polymorphism (runtime picks the actual type's version).
// new merely hides the base member (compile-time type decides which version runs).
Base overridden = new OverrideDerived();
overridden.Greet(); // "Hello from OverrideDerived" - the derived override wins

Base hidden = new HideDerived();
hidden.Greet(); // "Hello from Base" - static type (Base) decides, hiding is NOT polymorphic

Console.WriteLine();

// ---- 8. Sealed ----
// Rectangle seals its Move() override, so no further subclass of Rectangle
// can override Move() again - it is final at this point in the hierarchy.
var sealedRect = new Rectangle(1, 1);
sealedRect.Move();
// class Square : Rectangle { public override void Move() { } } // <- would not compile

// ---- Encapsulation: state is private, only accessible through methods/properties ----
class BankAccount
{
    public string Owner { get; }
    public decimal Balance { get; private set; }

    public BankAccount(string owner, decimal initialBalance)
    {
        Owner = owner;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        Balance -= amount;
    }

    // Overload 1: no arguments.
    public void Describe() => Console.WriteLine($"{Owner}: {Balance:C}");

    // Overload 2: same name, extra parameter - a different signature.
    public void Describe(string prefix) => Console.WriteLine($"{prefix} {Owner}: {Balance:C}");
}

// ---- Abstraction: a contract with no implementation details ----
interface IMovable
{
    void Move();
}

// ---- Inheritance: Shape is the base class for Circle and Rectangle ----
abstract class Shape : IMovable
{
    // static field: one copy shared across every Shape/Circle/Rectangle instance.
    public static int InstanceCount { get; private set; }

    protected Shape() => InstanceCount++; // runs for every derived constructor too

    public abstract double Area();

    // A virtual-by-default abstract method still allows shared behavior
    // in the base class (Describe uses the derived Area()).
    public void Describe() => Console.WriteLine($"{GetType().Name} area = {Area():F2}");

    public virtual void Move() => Console.WriteLine($"{GetType().Name} moves.");
}

class Circle : Shape
{
    private readonly double _radius;

    public Circle(double radius) => _radius = radius;

    public override double Area() => Math.PI * _radius * _radius;

    // Polymorphism: Circle overrides Move() with its own behavior.
    public override void Move() => Console.WriteLine("Circle rolls forward.");
}

class Rectangle : Shape
{
    private readonly double _width;
    private readonly double _height;

    // init-only property: settable via constructor or object initializer,
    // but not after the object is fully constructed.
    public string Label { get; init; } = "Rectangle";

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public override double Area() => _width * _height;

    // sealed override: Rectangle's subclasses can no longer override Move() again.
    public sealed override void Move() => Console.WriteLine($"{Label} slides sideways.");
}

// ---- A class unrelated to Shape can still satisfy IMovable ----
class Robot : IMovable
{
    private readonly string _name;

    public Robot(string name) => _name = name;

    public void Move() => Console.WriteLine($"Robot {_name} walks forward.");
}

// ---- override vs new (method hiding) ----
class Base
{
    public virtual void Greet() => Console.WriteLine("Hello from Base");
}

class OverrideDerived : Base
{
    // override: replaces Base.Greet() for every reference, even a Base-typed one.
    public override void Greet() => Console.WriteLine("Hello from OverrideDerived (override)");
}

class HideDerived : Base
{
    // new: declares an unrelated method that happens to share a name/signature.
    // A Base-typed reference still calls Base.Greet().
    public new void Greet() => Console.WriteLine("Hello from HideDerived (new/hiding)");
}
