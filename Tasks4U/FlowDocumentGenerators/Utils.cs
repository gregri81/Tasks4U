﻿using System;
using System.Globalization;
using Tasks4U.Converters;
using Tasks4U.Models;

namespace Tasks4U.FlowDocumentGenerators
{
    internal static class Utils
    {
        private static readonly FirstLineConverter _firstLineConverter = new FirstLineConverter();
        private static readonly SplitByCapitalLettersConverter splitByCapitalLettersConverter = new SplitByCapitalLettersConverter();
        private static readonly DateAndFrequencyConverter _dateAndFrequencyConverter = new DateAndFrequencyConverter();

        public static string SplitByCapitalLetters(object text) =>
            SplitByCapitalLetters(text?.ToString() ?? string.Empty);

        public static string SplitByCapitalLetters(string text) =>
           (string)splitByCapitalLettersConverter.Convert(text, typeof(string), string.Empty, CultureInfo.InvariantCulture);

        public static string GetTaskDescription(Task task) =>
            (string)_firstLineConverter.Convert(task.Description, typeof(string), null, CultureInfo.InvariantCulture);

        public static string GetDate(DateOnly date, Frequency taskFrequency, string? defaultValue = null)
        {
            var res = (string)_dateAndFrequencyConverter.Convert(new object[] { date, taskFrequency }, 
                                                                 typeof(object[]), 
                                                                 null, 
                                                                 CultureInfo.InvariantCulture);

            if (defaultValue != null && res == string.Empty)
                return defaultValue;

            return res;
        }
    }
}