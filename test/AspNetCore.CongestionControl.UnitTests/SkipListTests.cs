// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkipListTests.cs">
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
    using Moq;
    using SortedSet;
    using Xunit;

    public class SkipListTests
    {
        [Fact(DisplayName = "New Skip List Instance")]
        public void NewSkipListInstance()
        {
            // When skip list is initialized
            var skipList = new SkipList();

            // Then it should create the head node
            skipList.Head.Should().NotBeNull();

            // And it should initialize head node with maximum number of levels
            skipList.Head.Levels.Count.Should().Be(SkipList.SkipListMaxLevel);

            // And it should set the back link on the head node to null
            skipList.Head.Backward.Should().BeNull();

            // And it should set the tail node to null
            skipList.Tail.Should().BeNull();

            // And it should set the length to 0
            skipList.Length.Should().Be(0);

            // And it should set the levels to 1
            skipList.Levels.Should().Be(1);
        }

        [Fact(DisplayName = "Inserting First Node At Level 1")]
        public void InsertingFirstNodeAtLevel1()
        {
            // Given
            const int Score = 1;
            const string Element = "one";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);

            // When a node is inserted at level 1
            var newNode = skipList.Insert(Score, Element);

            // Then it should return the inserted node
            newNode.Should().NotBeNull();
            newNode.Score.Should().Be(Score);
            newNode.Element.Should().Be(Element);
            newNode.Backward.Should().BeNull();
            newNode.Levels.Count.Should().Be(1);

            // And it should make new node the tail node
            skipList.Tail.Should().Be(newNode);

            // And it should link head node at level 1 to new node
            skipList.Head.Levels[0].Forward.Should().Be(newNode);
            skipList.Head.Levels[0].Span.Should().Be(1);

            // And it should update length of the skip list to 1
            skipList.Length.Should().Be(1);

            // And it should update the level count to 1
            skipList.Levels.Should().Be(1);
        }

        [Fact(DisplayName = "Inserting First Node At Level 1 and Last Node At Level 2")]
        public void InsertingFirstNodeAtLevel1AndLastNodeAtLevel2()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const string FirstElement = "one";
            const string LastElement = "five";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();

            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);

            // When inserting first and last nodes
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);

            // Then it should return the inserted node
            lastNode.Should().NotBeNull();
            lastNode.Score.Should().Be(LastScore);
            lastNode.Element.Should().Be(LastElement);
            lastNode.Backward.Should().Be(firstNode);
            lastNode.Levels.Count.Should().Be(2);

            // And it should link last node to first node
            lastNode.Backward.Should().Be(firstNode);

            // And it should link first node to last node
            firstNode.Levels[0].Forward.Should().Be(lastNode);
            firstNode.Levels[0].Span.Should().Be(1);

            // And it should make last node the tail node
            skipList.Tail.Should().Be(lastNode);

            // And it should link head node at level 2 to last node
            skipList.Head.Levels[1].Forward.Should().Be(lastNode);
            skipList.Head.Levels[1].Span.Should().Be(2);

            // And it should update length to 2
            skipList.Length.Should().Be(2);

            // And it should update the level count to 2
            skipList.Levels.Should().Be(2);
        }

        [Fact(DisplayName = "Inserting First Node At Level 1, Last Node At Level 2 and Middle Node At Level 3")]
        public void InsertingFirstNodeAtLevel1LastNodeAtLevel2AndMiddleNodeAtLevel3()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2)
                .Returns(3);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);

            // When inserting all nodes
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);
            var middleNode = skipList.Insert(MiddleScore, MiddleElement);

            // Then it should return the inserted node
            middleNode.Should().NotBeNull();
            middleNode.Score.Should().Be(MiddleScore);
            middleNode.Element.Should().Be(MiddleElement);
            middleNode.Backward.Should().Be(firstNode);
            middleNode.Levels.Count.Should().Be(3);

            // And it should link first node to middle node
            firstNode.Levels[0].Forward.Should().Be(middleNode);
            firstNode.Levels[0].Span.Should().Be(1);

            // And it should link middle node to first node
            middleNode.Backward.Should().Be(firstNode);

            // And it should link middle node to last node at levels 1 and 2
            middleNode.Levels[0].Forward.Should().Be(lastNode);
            middleNode.Levels[0].Span.Should().Be(1);
            middleNode.Levels[1].Forward.Should().Be(lastNode);
            middleNode.Levels[1].Span.Should().Be(1);

            // And it should link last node to middle node
            lastNode.Backward.Should().Be(middleNode);

            // And it should make last node the tail node
            skipList.Tail.Should().Be(lastNode);

            // And it should link head node at levels 2 and 3 to middle node
            skipList.Head.Levels[1].Forward.Should().Be(middleNode);
            skipList.Head.Levels[1].Span.Should().Be(2);
            skipList.Head.Levels[2].Forward.Should().Be(middleNode);
            skipList.Head.Levels[2].Span.Should().Be(2);

            // And it should update length of the skip list to 3
            skipList.Length.Should().Be(3);

            // And it should update the level count to 3
            skipList.Levels.Should().Be(3);
        }

        [Fact(DisplayName = "Deleting the Only Node In The Skip List")]
        public void DeletingTheOnlyNodeInTheSkipList()
        {
            // Given
            const int Score = 1;
            const string Element = "one";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);

            skipList.Insert(Score, Element);

            // When node is deleted
            var deleteResult = skipList.Delete(Score, Element);

            // Then it should successfully delete node
            deleteResult.Should().BeTrue();

            // And it should set tail to null
            skipList.Tail.Should().BeNull();

            // And it should unlink head node at level 1 from deleted node
            skipList.Head.Levels[0].Forward.Should().BeNull();
            skipList.Head.Levels[0].Span.Should().Be(0);

            // And it should update length of the skip list to 1
            skipList.Length.Should().Be(0);

            // And it should update the level count to 1
            skipList.Levels.Should().Be(1);
        }

        [Fact(DisplayName = "Deleting the Middle Node From Skip List")]
        public void DeletingTheMiddleNodeFromSkipList()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2)
                .Returns(3);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);
            var middleNode = skipList.Insert(MiddleScore, MiddleElement);

            // When the middle node is deleted
            var deleteResult = skipList.Delete(MiddleScore, MiddleElement);

            // Then it should successfully delete node
            deleteResult.Should().BeTrue();

            // And it should link first node to last node
            firstNode.Levels[0].Forward.Should().Be(lastNode);
            firstNode.Levels[0].Span.Should().Be(1);

            // And it should link last node to first node
            lastNode.Backward.Should().Be(firstNode);

            // And it should make last node the tail node
            skipList.Tail.Should().Be(lastNode);

            // And it should link head node at level 2 to last node
            skipList.Head.Levels[1].Forward.Should().Be(lastNode);
            skipList.Head.Levels[1].Span.Should().Be(2);

            // And it should unlink head node at level 3 from deleted middle node
            skipList.Head.Levels[2].Forward.Should().BeNull();
            skipList.Head.Levels[2].Span.Should().Be(0);

            // And it should update length of the skip list to 2
            skipList.Length.Should().Be(2);

            // And it should update the level count to 2
            skipList.Levels.Should().Be(2);
        }

        [Fact(DisplayName = "Deleting the Last Node From Skip List")]
        public void DeletingTheLastNodeFromSkipList()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2)
                .Returns(3);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);
            var middleNode = skipList.Insert(MiddleScore, MiddleElement);

            // When the last node is deleted
            var deleteResult = skipList.Delete(LastScore, LastElement);

            // Then it should successfully delete node
            deleteResult.Should().BeTrue();

            // And it should unlink middle node from last node at levels 1 and 2
            middleNode.Levels[0].Forward.Should().BeNull();
            middleNode.Levels[0].Span.Should().Be(0);
            middleNode.Levels[1].Forward.Should().BeNull();
            middleNode.Levels[1].Span.Should().Be(0);

            // And it should make middle node the tail node
            skipList.Tail.Should().Be(middleNode);

            // And it should update length of the skip list to 2
            skipList.Length.Should().Be(2);

            // And it should update the level count to 3
            skipList.Levels.Should().Be(3);
        }

        [Fact(DisplayName = "Updating the Only Node in the Skip List")]
        public void UpdatingTheOnlyNodeInTheSkipList()
        {
            // Given
            const int Score = 1;
            const int NewScore = 2;
            const string Element = "one";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);
            skipList.Insert(Score, Element);

            // When skip list node is updated
            var updatedNode = skipList.Update(Score, Element, NewScore);

            // Then it should successfully update node
            updatedNode.Should().NotBeNull();
            updatedNode.Score.Should().Be(NewScore);
        }

        [Fact(DisplayName = "Updating the Middle Node in Skip List with Larger Score than Last Node")]
        public void UpdatingTheMiddleNodeInSkipListWithLargerScoreThanLastNode()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const int NewMiddleScore = 6;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2)
                .Returns(3)
                .Returns(3);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);
            var middleNode = skipList.Insert(MiddleScore, MiddleElement);

            // When the middle node is updated
            var updatedNode = skipList.Update(MiddleScore, MiddleElement, NewMiddleScore);

            // Then it should successfully update node
            updatedNode.Should().NotBeNull();

            // And it should link first node to last node
            firstNode.Levels[0].Forward.Should().Be(lastNode);
            firstNode.Levels[0].Span.Should().Be(1);

            // And it should link last node to first node
            lastNode.Backward.Should().Be(firstNode);

            // And it should link last node to updated node at levels 1 and 2
            lastNode.Levels[0].Forward.Should().Be(updatedNode);
            lastNode.Levels[0].Span.Should().Be(1);
            lastNode.Levels[1].Forward.Should().Be(updatedNode);
            lastNode.Levels[1].Span.Should().Be(1);

            // And it should link updated node to last node
            updatedNode.Backward.Should().Be(lastNode);

            // And it should make updated node the tail node
            skipList.Tail.Should().Be(updatedNode);

            // And it should link head node at level 2 to last node
            skipList.Head.Levels[1].Forward.Should().Be(lastNode);
            skipList.Head.Levels[1].Span.Should().Be(2);

            // And it should unlink head node at level 3 from updated node
            skipList.Head.Levels[2].Forward.Should().Be(updatedNode);
            skipList.Head.Levels[2].Span.Should().Be(3);

            // And it should update length of skip list to 3
            skipList.Length.Should().Be(3);

            // And it should update the level count to 3
            skipList.Levels.Should().Be(3);
        }

        [Fact(DisplayName = "Deleting a Range of Nodes by Score From Skip List")]
        public void DeletingARangeOfNodesByScoreFromSkipList()
        {
            // Given
            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            var nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                .Returns(1)
                .Returns(2)
                .Returns(3);

            var skipList = new SkipList(nodeLevelGeneratorMock.Object);
            var firstNode = skipList.Insert(FirstScore, FirstElement);
            var lastNode = skipList.Insert(LastScore, LastElement);
            var middleNode = skipList.Insert(MiddleScore, MiddleElement);

            // When a range of nodes is deleted
            var nodesDeleted = skipList.DeleteRangeByScore(new SkipListRange
            {
                Min = FirstScore,
                Max = MiddleScore
            });

            // Then it should successfully delete nodes in range
            nodesDeleted.Should().Be(2);

            // And it should make last node the tail node
            skipList.Tail.Should().Be(lastNode);

            // And it should link head node at levels 1 and 2 to last node
            skipList.Head.Levels[0].Forward.Should().Be(lastNode);
            skipList.Head.Levels[0].Span.Should().Be(1);
            skipList.Head.Levels[1].Forward.Should().Be(lastNode);
            skipList.Head.Levels[1].Span.Should().Be(1);

            // And it should unlink head node at levels 3 from deleted middle node
            skipList.Head.Levels[2].Forward.Should().BeNull();
            skipList.Head.Levels[2].Span.Should().Be(0);

            // And it should update length of the skip list to 1
            skipList.Length.Should().Be(1);

            // And it should update the level count to 2
            skipList.Levels.Should().Be(2);
        }
    }
}
