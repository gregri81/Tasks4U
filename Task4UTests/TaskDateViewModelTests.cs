﻿using System;
using System.Collections.Generic;
using Tasks4U.Models;
using Tasks4U.ViewModels;
using static Tasks4U.ViewModels.TaskDateViewModel;

namespace Task4UTests
{
    [TestClass]
    public class TaskDateViewModelTests
    {
        [DataTestMethod]
        [DataRow(Frequency.Once)]
        [DataRow(Frequency.EveryWeek)]
        [DataRow(Frequency.EveryYear)]
        [DataRow(Frequency.EveryMonth)]
        public void TestFrequencyProperties(Frequency frequency)
        {
            var isOnceFrequency = false;
            var isEveryWeekFrequency = false;
            var isEveryMonthFrequency = false;
            var isEveryYearFrequency = false;
            var date = DateTime.MinValue;

            var taskDateViewModel = new TaskDateViewModel();

            taskDateViewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(TaskDateViewModel.IsOnceFrequency):
                        isOnceFrequency = taskDateViewModel.IsOnceFrequency;
                        break;
                    case nameof(TaskDateViewModel.IsEveryWeekFrequency):
                        isEveryWeekFrequency = taskDateViewModel.IsEveryWeekFrequency;
                        break;
                    case nameof(TaskDateViewModel.IsEveryMonthFrequency):
                        isEveryMonthFrequency = taskDateViewModel.IsEveryMonthFrequency;
                        break;
                    case nameof(TaskDateViewModel.IsEveryYearFrequency):
                        isEveryYearFrequency = taskDateViewModel.IsEveryYearFrequency;
                        break;
                }
            };

            taskDateViewModel.TaskFrequency = frequency;

            Assert.IsTrue(taskDateViewModel.IsOnceFrequency == (frequency == Frequency.Once));
            Assert.IsTrue(taskDateViewModel.IsEveryWeekFrequency == (frequency == Frequency.EveryWeek));
            Assert.IsTrue(taskDateViewModel.IsEveryMonthFrequency == (frequency == Frequency.EveryMonth));
            Assert.IsTrue(taskDateViewModel.IsEveryYearFrequency == (frequency == Frequency.EveryYear));
        }

        [TestMethod]
        public void TestDateProperties()
        {
            var date = DateTime.MinValue;
            var weekDay = DayOfWeek.Sunday;
            var day = 0;
            var month = MonthOfYear.January;

            var taskDateViewModel = new TaskDateViewModel();

            taskDateViewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(TaskDateViewModel.Date):
                        date = taskDateViewModel.Date;
                        break;
                    case nameof(TaskDateViewModel.WeekDay):
                        weekDay = taskDateViewModel.WeekDay;
                        break;
                    case nameof(TaskDateViewModel.Day):
                        day = taskDateViewModel.Day;
                        break;
                    case nameof(TaskDateViewModel.Month):
                        month = taskDateViewModel.Month;
                        break;
                }
            };

            var expectedDate = new DateTime(1, 2, 3);
            var expectedWeekDay = DayOfWeek.Wednesday;
            var expectedDay = 5;
            var expectedMonth = MonthOfYear.May;

            taskDateViewModel.Date = expectedDate;
            taskDateViewModel.WeekDay = expectedWeekDay;
            taskDateViewModel.Day = expectedDay;
            taskDateViewModel.Month = expectedMonth;

            Assert.AreEqual(expectedDate, taskDateViewModel.Date);
            Assert.AreEqual(expectedWeekDay, taskDateViewModel.WeekDay);
            Assert.AreEqual(expectedMonth, taskDateViewModel.Month);
            Assert.AreEqual(expectedDay, taskDateViewModel.Day);
        }

        [DataTestMethod]
        [DataRow(MonthOfYear.January, 31)]
        [DataRow(MonthOfYear.February, 28)]
        [DataRow(MonthOfYear.March, 31)]
        [DataRow(MonthOfYear.April, 30)]
        [DataRow(MonthOfYear.May, 31)]
        [DataRow(MonthOfYear.June, 30)]
        [DataRow(MonthOfYear.July, 31)]
        [DataRow(MonthOfYear.August, 31)]
        [DataRow(MonthOfYear.September, 30)]
        [DataRow(MonthOfYear.October, 31)]
        [DataRow(MonthOfYear.November, 30)]
        [DataRow(MonthOfYear.December, 31)]
        public void TestDaysInMonth(MonthOfYear month, int numOfDays)
        {
            var daysInMonth = new int[0];

            var taskDateViewModel = new TaskDateViewModel();

            taskDateViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TaskDateViewModel.DaysInMonth))
                    daysInMonth = taskDateViewModel.DaysInMonth.ToArray();
            };

            taskDateViewModel.Month = month;

            var expectedDaysInMonth = Enumerable.Range(1, numOfDays).ToArray();

            CollectionAssert.AreEqual(expectedDaysInMonth, daysInMonth);
        }

        [TestMethod]
        public void TestTaskDate()
        {            
            var today = DateOnly.FromDateTime(DateTime.Today);

            var taskDateViewModel = new TaskDateViewModel();
            
            // Once
            taskDateViewModel.TaskFrequency = Frequency.Once;
            taskDateViewModel.Date = new DateTime(today.Year, today.Month, today.Day);
            Assert.AreEqual(today, taskDateViewModel.TaskDate);

            // Every Week
            taskDateViewModel.TaskFrequency = Frequency.EveryWeek;
            taskDateViewModel.WeekDay = DayOfWeek.Monday;

            var nextMondayDate = today.AddDays((DayOfWeek.Monday - today.DayOfWeek + 7) % 7);
            taskDateViewModel.TaskFrequency = Frequency.EveryWeek;
            Assert.AreEqual(nextMondayDate, taskDateViewModel.TaskDate);

            // Every Month
            taskDateViewModel.TaskFrequency = Frequency.EveryMonth;
            taskDateViewModel.Day = 15;

            var next15Date = new DateOnly(today.Year, today.Month, 15);

            if (next15Date < today)
            {
                next15Date = next15Date.AddMonths(1);
                next15Date = new DateOnly(next15Date.Year, next15Date.Month, 15);
            }

            Assert.AreEqual(next15Date, taskDateViewModel.TaskDate);

            // Every Year
            taskDateViewModel.TaskFrequency = Frequency.EveryYear;
            taskDateViewModel.Month = MonthOfYear.February;
            taskDateViewModel.DayInMonth = 28;

            var nextDate = new DateOnly(today.Year, 2, 28);

            if (nextDate < today)
                nextDate = nextDate.AddYears(1);

            Assert.AreEqual(nextDate, taskDateViewModel.TaskDate);
        }
    }
}
