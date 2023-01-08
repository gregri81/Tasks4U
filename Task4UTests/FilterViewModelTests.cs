using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks4U.ViewModels;

namespace Task4UTests
{
    [TestClass]
    public class FilterViewModelTests
    {
        [TestMethod]
        public void FilterTypeSelectionTest() 
        {
            var filterViewModel = new FilterViewModel();
            var selectedFilter = FilterViewModel.FilterType.Subject;
            var isTextFilter = false;
            var isDeskFilter = false;
            var isStatusFilter = false;
            var isDateFilter = false;

            filterViewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(FilterViewModel.SelectedFilter):
                        selectedFilter = filterViewModel.SelectedFilter;
                        break;
                    case nameof(filterViewModel.IsTextFilter):
                        isTextFilter = filterViewModel.IsTextFilter;
                        break;                   
                    case nameof(filterViewModel.IsStatusFilter):
                        isStatusFilter = filterViewModel.IsStatusFilter;
                        break;
                    case nameof(filterViewModel.IsDeskFilter):
                        isDeskFilter = filterViewModel.IsDeskFilter;
                        break;
                    case nameof(filterViewModel.IsDateFilter):
                        isDateFilter = filterViewModel.IsDateFilter;
                        break;
                }
            };

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Date;

            Assert.AreEqual(FilterViewModel.FilterType.Date, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsTrue(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Subject;

            Assert.AreEqual(FilterViewModel.FilterType.Subject, selectedFilter);
            Assert.IsTrue(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Status;

            Assert.AreEqual(FilterViewModel.FilterType.Status, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsTrue(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.RelatedTo;

            Assert.AreEqual(FilterViewModel.FilterType.RelatedTo, selectedFilter);
            Assert.IsTrue(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Desk;

            Assert.AreEqual(FilterViewModel.FilterType.Desk, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsTrue(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Description;

            Assert.AreEqual(FilterViewModel.FilterType.Description, selectedFilter);
            Assert.IsTrue(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.None;

            Assert.AreEqual(FilterViewModel.FilterType.None, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);
        }

        public void SubjectFilterTest()
        {
            var filterViewModel = new FilterViewModel();

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Subject;
        }
    }
}
