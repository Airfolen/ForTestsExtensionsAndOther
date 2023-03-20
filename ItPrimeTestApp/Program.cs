// See https://aka.ms/new-console-template for more information

using ItPrimeTestApp;

var checker = new NumberChecker();
var count = checker.CalculateBeautyCount();

Console.WriteLine($"Result count: {count}");