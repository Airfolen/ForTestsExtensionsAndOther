using System.Globalization;

namespace ItPrimeTestApp;

public class NumberChecker
{
    /// <summary>
    /// Система исчисления
    /// </summary>
    private const int Base = 13;

    /// <summary>
    /// Количество цифр
    /// </summary>
    private const int DigitsAmount = 13;

    /// <summary>
    /// Все символы для тринадцатиричной системы исчисления
    /// </summary>
    private static readonly char[] BaseChars =
    {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        'A',
        'B',
        'C'
    };

    /// <summary>
    /// Вычисление количества красивых чисел
    /// Алгоритм: находим максимальное количество вариаций сумм для одной из половин
    /// И для этих сумм считаем количество их появления от 0 до максимальной суммы: g(0), g(1) ... g(MaxSum)
    /// Тк половины две, то g(0)^2+g(1)^2 ... g(maxSum)^2
    /// И тк для числа в середине может быть количество вариаций = системе исчесления (13), то
    /// Количество красивых чисел будет равно: система исчесления (13) * (g(0)^2+g(1)^2 ... g(maxSum)^2)
    /// </summary>
    public long CalculateBeautyCount()
    {
        var halfDigitsAmount = DigitsAmount / 2;

        var biggestSum = (Base - 1) * halfDigitsAmount;
        var sumStorage = new long[biggestSum + 1];
        var maxNumber = Math.Pow(Base, halfDigitsAmount) - 1;

        for (var number = 0; number <= maxNumber; number++)
        {
            var chars = IntToString(number).Select(character => character).ToArray();
            var digits = chars.Select(character => int.Parse(character.ToString(), NumberStyles.HexNumber));
            var sum = digits.Sum(a => a);
            sumStorage[sum] += 1;
        }

        var result = sumStorage.Sum(sum => sum * sum) * Base;

        return result;
    }

    /// <summary>
    /// Конвертация десятичного числа в триндцетиричное число в строковом виде
    /// </summary>
    private string IntToString(int value)
    {
        var result = string.Empty;
        var targetBase = BaseChars.Length;

        do
        {
            result = BaseChars[value % targetBase] + result;
            value /= targetBase;
        } while (value > 0);

        return result;
    }
}