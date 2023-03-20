// See https://aka.ms/new-console-template for more information

// test .net 6 features


using ForTests;
using ForTests.Extensions;

var strings = new[] {"a", "b", "c", "d", "e", "f", "g"};
var numbers = new[] {1, 2, 3, 4, 5, 6};
var peoples = new[]
{
    new People("Name 1", 23),
    new People("Name 2", 94),
    new People("Name 3", 35),
    new People("Name 4", 94),
};
var otherPeoples = new[]
{
    new People("Name 1", 23),
    new People("Name 2", 94),
    new People("Name 10", 5),
};


//first
var chunked = strings.Chunk(2);

//second
var result = chunked.TryGetNonEnumeratedCount(out var count);

//third
IEnumerable<(string Name, int Number)> zipped = strings.Zip(numbers);

//fourth
var oldest = peoples.MaxBy(a => a.Age);
var youngest = peoples.MinBy(a => a.Age);
var distinctTest = peoples.DistinctBy(a => a.Age);
var union = peoples.UnionBy(otherPeoples, a => a.Age);
var intersect = peoples.IntersectBy(otherPeoples.Select(a => a.Name), a => a.Name);
var except = peoples.ExceptBy(otherPeoples.Select(a => a.Name), a => a.Name);


//fifth
var slice = peoples.Take(1..3);
var lastTwo = peoples.Take(^2..);


// custom foreach feature, same speed without allocation
foreach (var i in 0..10)
    Console.WriteLine($"Cool looking foreach {i}");

foreach (var i in 10)
    Console.WriteLine($"Other way for cool looking foreach {i}");

Console.WriteLine("Hello, World!");


namespace ForTests
{
    public class People
    {
        public People(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}