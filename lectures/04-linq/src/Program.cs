// Sample - LINQ
// Run: dotnet run (from this src/ folder)
// Mirrors the README section by section.

var products = new List<Product>
{
    new("Laptop", 1500m, "Electronics"),
    new("Mouse", 25m, "Electronics"),
    new("Desk", 300m, "Furniture"),
    new("Chair", 150m, "Furniture"),
};

Section("1. What LINQ is");
{
    var numbers = new List<int> { 5, 2, 8, 1, 9, 3 };

    var evensLoop = new List<int>();
    foreach (var n in numbers)
    {
        if (n % 2 == 0) evensLoop.Add(n);
    }
    var evensLinq = numbers.Where(n => n % 2 == 0).ToList();
    Console.WriteLine($"Loop version: {string.Join(", ", evensLoop)}");
    Console.WriteLine($"LINQ version: {string.Join(", ", evensLinq)}");

    var cheapMethod = products.Where(p => p.Price < 500).OrderBy(p => p.Price).Select(p => p.Name);
    var cheapQuery = from p in products where p.Price < 500 orderby p.Price select p.Name;
    Console.WriteLine($"Method syntax: {string.Join(", ", cheapMethod)}");
    Console.WriteLine($"Query syntax : {string.Join(", ", cheapQuery)}");
}

Section("2. Where");
{
    var electronics = products.Where(p => p.Category == "Electronics");
    var indexed = products.Where((p, index) => index % 2 == 0);
    Console.WriteLine($"Electronics: {string.Join(", ", electronics.Select(p => p.Name))}");
    Console.WriteLine($"Even positions: {string.Join(", ", indexed.Select(p => p.Name))}");
}

Section("3. Select / SelectMany");
{
    var names = products.Select(p => p.Name);
    var summaries = products.Select(p => $"{p.Name}: {p.Price:C}");
    Console.WriteLine($"Names: {string.Join(", ", names)}");
    Console.WriteLine($"Summaries: {string.Join(" | ", summaries)}");

    var orders = new List<Order>
    {
        new("Alice", new List<string> { "Book", "Pen" }),
        new("Bob", new List<string> { "Laptop" }),
    };

    var allItemsFlat = orders.SelectMany(o => o.Items);
    Console.WriteLine($"Flattened items: {string.Join(", ", allItemsFlat)}");

    var customerItems = orders.SelectMany(o => o.Items, (order, item) => $"{order.Customer} bought {item}");
    foreach (var line in customerItems) Console.WriteLine($"  {line}");
}

Section("4. OrderBy / ThenBy");
{
    var sorted = products
        .OrderBy(p => p.Category)
        .ThenByDescending(p => p.Price)
        .Select(p => $"{p.Category}/{p.Name}: {p.Price:C}");
    foreach (var line in sorted) Console.WriteLine($"  {line}");
}

Section("5. GroupBy");
{
    var byCategory = products.GroupBy(p => p.Category);
    foreach (var group in byCategory)
    {
        Console.WriteLine($"{group.Key}: {group.Count()} item(s), total {group.Sum(p => p.Price):C}");
        foreach (var p in group) Console.WriteLine($"  - {p.Name}");
    }

    var totals = products.GroupBy(
        p => p.Category,
        (category, items) => new { Category = category, Total = items.Sum(p => p.Price) });
    foreach (var t in totals) Console.WriteLine($"  {t.Category} total = {t.Total:C}");
}

Section("6. Join / GroupJoin");
{
    var categories = new List<Category>
    {
        new("Electronics", "Dana"),
        new("Furniture", "Sam"),
    };

    var joined = products.Join(
        categories,
        product => product.Category,
        category => category.Name,
        (product, category) => $"{product.Name} is managed by {category.Manager}");
    foreach (var line in joined) Console.WriteLine($"  {line}");

    var grouped = categories.GroupJoin(
        products,
        category => category.Name,
        product => product.Category,
        (category, matchingProducts) => new
        {
            category.Name,
            Products = matchingProducts.Select(p => p.Name).ToList(),
        });
    foreach (var g in grouped) Console.WriteLine($"  {g.Name}: {string.Join(", ", g.Products)}");
}

Section("7. Set operations");
{
    var a = new[] { 1, 2, 3, 4 };
    var b = new[] { 3, 4, 5, 6 };
    Console.WriteLine($"Distinct: {string.Join(", ", a.Distinct())}");
    Console.WriteLine($"Union: {string.Join(", ", a.Union(b))}");
    Console.WriteLine($"Intersect: {string.Join(", ", a.Intersect(b))}");
    Console.WriteLine($"Except: {string.Join(", ", a.Except(b))}");
}

Section("8. Aggregation");
{
    var prices = products.Select(p => p.Price);
    Console.WriteLine($"Count: {products.Count()}");
    Console.WriteLine($"Count(>200): {products.Count(p => p.Price > 200)}");
    Console.WriteLine($"Sum: {prices.Sum():C}");
    Console.WriteLine($"Min: {prices.Min():C}");
    Console.WriteLine($"Max: {prices.Max():C}");
    Console.WriteLine($"Average: {prices.Average():C}");

    var totalWithFee = prices.Aggregate(0m, (runningTotal, price) => runningTotal + price * 1.05m);
    var namesJoined = products.Aggregate("", (acc, p) => acc == "" ? p.Name : $"{acc}, {p.Name}");
    Console.WriteLine($"Total with 5% fee: {totalWithFee:C}");
    Console.WriteLine($"Names joined via Aggregate: {namesJoined}");
}

Section("9. Element operators / quantifiers");
{
    Console.WriteLine($"First Electronics: {products.First(p => p.Category == "Electronics").Name}");
    Console.WriteLine($"FirstOrDefault Toys: {products.FirstOrDefault(p => p.Category == "Toys")?.Name ?? "null"}");
    Console.WriteLine($"Single Desk: {products.Single(p => p.Name == "Desk").Name}");
    Console.WriteLine($"SingleOrDefault Toys: {products.SingleOrDefault(p => p.Category == "Toys")?.Name ?? "null"}");

    Console.WriteLine($"Any: {products.Any()}");
    Console.WriteLine($"Any(>1000): {products.Any(p => p.Price > 1000)}");
    Console.WriteLine($"All(>0): {products.All(p => p.Price > 0)}");
    Console.WriteLine($"Contains first: {products.Contains(products[0])}");
}

Section("10. Partitioning");
{
    var nums = Enumerable.Range(1, 10).ToArray();
    Console.WriteLine($"Skip(3): {string.Join(", ", nums.Skip(3))}");
    Console.WriteLine($"Take(3): {string.Join(", ", nums.Take(3))}");
    Console.WriteLine($"Skip(3).Take(3): {string.Join(", ", nums.Skip(3).Take(3))}");
    Console.WriteLine($"TakeWhile(<5): {string.Join(", ", nums.TakeWhile(n => n < 5))}");
    Console.WriteLine($"SkipWhile(<5): {string.Join(", ", nums.SkipWhile(n => n < 5))}");
    foreach (var chunk in nums.Chunk(3)) Console.WriteLine($"  chunk: {string.Join(", ", chunk)}");
}

Section("11. Conversion");
{
    var list = products.Where(p => p.Price > 100).ToList();
    var array = products.ToArray();
    var byName = products.ToDictionary(p => p.Name, p => p.Price);
    var categorySet = products.Select(p => p.Category).ToHashSet();
    var byCategoryLookup = products.ToLookup(p => p.Category);

    Console.WriteLine($"ToList (>100): {string.Join(", ", list.Select(p => p.Name))}");
    Console.WriteLine($"ToArray count: {array.Length}");
    Console.WriteLine($"ToDictionary[Laptop]: {byName["Laptop"]:C}");
    Console.WriteLine($"ToHashSet: {string.Join(", ", categorySet)}");
    Console.WriteLine($"ToLookup[Electronics]: {string.Join(", ", byCategoryLookup["Electronics"].Select(p => p.Name))}");
}

Section("12. Deferred execution");
{
    var numbers = new List<int> { 1, 2, 3 };
    var query = numbers.Where(n =>
    {
        Console.WriteLine($"  Checking {n}");
        return n > 1;
    });

    Console.WriteLine("Query built, nothing printed yet");
    foreach (var n in query) Console.WriteLine($"  Got {n}");

    var list = new List<int> { 1, 2, 3 };
    var liveQuery = list.Where(n => n > 1);
    list.Add(4);
    Console.WriteLine($"Live query after Add(4): {string.Join(" ", liveQuery)}");

    var evalCount = 0;
    var expensive = numbers.Where(n =>
    {
        evalCount++;
        return n > 1;
    });
    var count = expensive.Count();
    var materialized = expensive.ToList();
    Console.WriteLine($"Predicate evaluated {evalCount} times across two enumerations (would be {numbers.Count} for one)");

    var evalCountFixed = 0;
    var fixedQuery = numbers.Where(n =>
    {
        evalCountFixed++;
        return n > 1;
    }).ToList(); // materialize once
    var countFixed = fixedQuery.Count;
    var materializedFixed = fixedQuery.ToList();
    Console.WriteLine($"Fixed version evaluated {evalCountFixed} times (materialized once, then reused)");
}

Section("13. IEnumerable<T> vs IQueryable<T> (conceptual)");
{
    // No real database here - this just illustrates that the same syntax
    // produces an in-memory IEnumerable<T> when the source is a List<T>.
    IEnumerable<Product> inMemory = products.Where(p => p.Price > 100);
    Console.WriteLine($"In-memory filter: {string.Join(", ", inMemory.Select(p => p.Name))}");
    Console.WriteLine("(IQueryable<T> would translate this same Where(...) into SQL against a database - see EF Core.)");
}

Section("Exercise 5 solution - Join with Employee/Department");
{
    var departments = new List<Department> { new(1, "Engineering"), new(2, "Sales") };
    var employees = new List<Employee> { new("Alice", 1), new("Bob", 2), new("Carol", 1) };

    var employeeDepartments = employees.Join(
        departments,
        e => e.DepartmentId,
        d => d.Id,
        (e, d) => $"{e.Name} works in {d.Name}");

    foreach (var line in employeeDepartments) Console.WriteLine($"  {line}");
}

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"=== {title} ===");
}

record Product(string Name, decimal Price, string Category);
record Order(string Customer, List<string> Items);
record Category(string Name, string Manager);
record Employee(string Name, int DepartmentId);
record Department(int Id, string Name);
