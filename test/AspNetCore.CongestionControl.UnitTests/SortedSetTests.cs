namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;

    class SortedSetTests
    {
        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_inserting_new_score_and_element
        {
            Because of = () =>
            {
                _result = _sortedSet.Insert(1, "one");
            };

            It should_successfully_insert = () =>
            {
                _result.ShouldBeTrue();
                _sortedSet.Length.ShouldEqual(1);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_inserting_existing_score_with_new_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Insert(1, "two");
            };

            It should_successfully_insert = () =>
            {
                _result.ShouldBeTrue();
                _sortedSet.Length.ShouldEqual(2);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Negative Test")]
        public class When_inserting_a_duplicate
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Insert(1, "one");
            };

            It should_not_insert = () =>
            {
                _result.ShouldBeFalse();
                _sortedSet.Length.ShouldEqual(1);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_deleting_by_existing_score_and_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Delete(1, "one");
            };

            It should_successfully_delete = () =>
            {
                _result.ShouldBeTrue();
                _sortedSet.Length.ShouldEqual(0);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Negative Test")]
        public class When_deleting_by_nonexisting_score_and_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Delete(1, "two");
            };

            It should_not_delete = () =>
            {
                _result.ShouldBeFalse();
                _sortedSet.Length.ShouldEqual(1);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_deleting_by_existing_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Delete("one");
            };

            It should_successfully_delete = () =>
            {
                _result.ShouldBeTrue();
                _sortedSet.Length.ShouldEqual(0);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Negative Test")]
        public class When_deleting_by_nonexisting_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Delete("two");
            };

            It should_not_delete = () =>
            {
                _result.ShouldBeFalse();
                _sortedSet.Length.ShouldEqual(1);
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_updating_existing_score_and_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Update(1, "one", 2);
            };

            It should_successfully_update = () =>
            {
                _result.ShouldBeTrue();
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Negative Test")]
        public class When_updating_nonexisting_score_and_element
        {
            Because of = () =>
            {
                _result = _sortedSet.Update(1, "one", 2);
            };

            It should_not_update = () =>
            {
                _result.ShouldBeFalse();
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Negative Test")]
        public class When_updating_existing_score_and_with_invalid_element
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
            };

            Because of = () =>
            {
                _result = _sortedSet.Update(1, "two", 2);
            };

            It should_not_update = () =>
            {
                _result.ShouldBeFalse();
            };

            static bool _result;
            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }

        [Subject(typeof(SortedSet.SortedSet), "Sorted Set"), Tags("Positive Test")]
        public class When_deleting_a_range_of_items
        {
            Establish context = () =>
            {
                _sortedSet.Insert(1, "one");
                _sortedSet.Insert(2, "two");
                _sortedSet.Insert(3, "three");
                _sortedSet.Insert(4, "four");
            };

            Because of = () =>
            {
                _sortedSet.DeleteRangeByScore(1, 3);
            };

            It should_successfully_update = () =>
            {
                _sortedSet.Length.ShouldEqual(1);
            };

            static SortedSet.SortedSet _sortedSet = new SortedSet.SortedSet();
        }
    }
}