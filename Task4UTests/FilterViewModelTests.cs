using Tasks4U.ViewModels;
using Task = Tasks4U.Models.Task;
using Tasks4U.Models;
using TaskStatus = Tasks4U.Models.TaskStatus;

namespace Task4UTests
{
    [TestClass]
    public class FilterViewModelTests
    {
        [TestMethod]
        public void FilterTypeSelectionTest() 
        {
            var filterViewModel = new FilterViewModel();
            var selectedFilter = FilterViewModel.FilterType.Text;
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

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Text;

            Assert.AreEqual(FilterViewModel.FilterType.Text, selectedFilter);
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

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Desk;

            Assert.AreEqual(FilterViewModel.FilterType.Desk, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsTrue(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);

            filterViewModel.SelectedFilter = FilterViewModel.FilterType.None;

            Assert.AreEqual(FilterViewModel.FilterType.None, selectedFilter);
            Assert.IsFalse(isTextFilter);
            Assert.IsFalse(isDeskFilter);
            Assert.IsFalse(isStatusFilter);
            Assert.IsFalse(isDateFilter);
        }

        [TestMethod]
        public void FilterTest()
        {
            var filterViewModel = new FilterViewModel();
            
            var today = DateOnly.FromDateTime(DateTime.Today);
            var yesterday = today.AddDays(-1);
            var twoDaysAgo = today.AddDays(-2);
            var tomorrow = today.AddDays(1);

            var tasks = new Task[]
            {
                new Task("task0") { Description = "...text...", Desk = Desk.Canada, FinalDate = yesterday},
                new Task("task1") { Name = "...text...", Status = TaskStatus.InProgress, FinalDate = tomorrow},
                new Task("task2") { RelatedTo = "...text...", Status = TaskStatus.InProgress, FinalDate = twoDaysAgo},
                new Task("task3") { Description = "...", Name = "...", RelatedTo = "...", IntermediateDate = today}
            };

            bool[] isFilteredOut = new bool[tasks.Length];
                
            filterViewModel.IsFilterChanged += () =>
            {
                for (var i = 0; i < tasks.Length; i++)
                    isFilteredOut[i] = !filterViewModel.IsTaskFilteredIn(tasks[i]);
            };

            // Text filter
            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Text;
            filterViewModel.Text = "text";

            Assert.IsFalse(isFilteredOut[0]);
            Assert.IsFalse(isFilteredOut[1]);
            Assert.IsFalse(isFilteredOut[2]);
            Assert.IsTrue(isFilteredOut[3]);

            // Desk filter
            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Desk;
            filterViewModel.Desk = Desk.Canada;

            Assert.IsFalse(isFilteredOut[0]);
            Assert.IsTrue(isFilteredOut[1]);
            Assert.IsTrue(isFilteredOut[2]);
            Assert.IsTrue(isFilteredOut[3]);


            // Status filter
            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Status;
            filterViewModel.Status = TaskStatus.InProgress;

            Assert.IsTrue(isFilteredOut[0]);
            Assert.IsFalse(isFilteredOut[1]);
            Assert.IsFalse(isFilteredOut[2]);
            Assert.IsTrue(isFilteredOut[3]);

            // Date filter
            filterViewModel.SelectedFilter = FilterViewModel.FilterType.Date;
            filterViewModel.StartDateText = yesterday.ToString(TaskDateViewModel.DateFormat);
            filterViewModel.EndDateText = today.ToString(TaskDateViewModel.DateFormat);

            Assert.IsFalse(isFilteredOut[0]);
            Assert.IsTrue(isFilteredOut[1]);
            Assert.IsTrue(isFilteredOut[2]);
            Assert.IsFalse(isFilteredOut[3]);
        }
    }
}
