// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedSetTests.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class SortedSetTests
    {
        [Fact(DisplayName = "Inserting New Score and Element")]
        public void InsertingNewScoreAndElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();

            // When a score is inserted
            var result = sortedSet.Insert(1, "one");

            // Then it should successfully insert
            result.Should().BeTrue();
            sortedSet.Length.Should().Be(1);
        }

        [Fact(DisplayName = "Inserting Existing Score with New Element")]
        public void InsertingExistingScoreWithNewElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When an existing score is inserted using new element
            var result = sortedSet.Insert(1, "two");

            // Then it should successfully insert
            result.Should().BeTrue();
            sortedSet.Length.Should().Be(2);
        }

        [Fact(DisplayName = "Inserting a Duplicate")]
        public void InsertingADuplicate()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When a duplicate is inserted
            var result = sortedSet.Insert(1, "one");

            // Then it should not insert
            result.Should().BeFalse();
            sortedSet.Length.Should().Be(1);
        }

        [Fact(DisplayName = "Deleting by Existing Score and Element")]
        public void DeletingByExistingScoreAndElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When existing score and element are deleted
            var result = sortedSet.Delete(1, "one");

            // Then it should successfully delete
            result.Should().BeTrue();
            sortedSet.Length.Should().Be(0);
        }

        [Fact(DisplayName = "Deleting by Non-Existing Score and Element")]
        public void DeletingByNonExistingScoreAndElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When deleting a non-existing score and element
            var result = sortedSet.Delete(1, "two");

            // Then it should not delete
            result.Should().BeFalse();
            sortedSet.Length.Should().Be(1);
        }

        [Fact(DisplayName = "Deleting by Existing Element")]
        public void DeletingByExistingElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When an existing element is deleted
            var result = sortedSet.Delete("one");

            // Then it should successfully delete
            result.Should().BeTrue();
            sortedSet.Length.Should().Be(0);
        }

        [Fact(DisplayName = "Deleting by Non-Existing Element")]
        public void DeletingByNonExistingElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When a non-existing element is deleted
            var result = sortedSet.Delete("two");

            // Then it should not delete
            result.Should().BeFalse();
            sortedSet.Length.Should().Be(1);
        }

        [Fact(DisplayName = "Updating Existing Score and Element")]
        public void UpdatingExistingScoreAndElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When an existing score and element are updated
            var result = sortedSet.Update(1, "one", 2);

            // Then it should successfully update
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "Updating Non-Existing Score and Element")]
        public void UpdatingNonExistingScoreAndElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();

            // When a non-existing score/element are updated
            var result = sortedSet.Update(1, "one", 2);

            // Then it should not update
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Updating Existing Score with Invalid Element")]
        public void UpdatingExistingScoreWithInvalidElement()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");

            // When an existing score is updated with invalid element
            var result = sortedSet.Update(1, "two", 2);

            // Then it should not update
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Deleting a Range of Items")]
        public void DeletingARangeOfItems()
        {
            // Given
            var sortedSet = new SortedSet.SortedSet();
            sortedSet.Insert(1, "one");
            sortedSet.Insert(2, "two");
            sortedSet.Insert(3, "three");
            sortedSet.Insert(4, "four");

            // When deleting a range of items
            sortedSet.DeleteRangeByScore(1, 3);

            // It should successfully delete
            sortedSet.Length.Should().Be(1);
        }
    }
}
