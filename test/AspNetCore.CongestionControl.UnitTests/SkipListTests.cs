namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;
    using Moq;
    using SortedSet;
    using It = Machine.Specifications.It;

    class SkipListTests
    {
        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_creating_new_skip_list_instance
        {
            Because of = () =>
            {
                _skipList = new SkipList();
            };

            It should_create_the_head_node = () =>
            {
                _skipList.Head.ShouldNotBeNull();
            };

            It should_initialize_head_node_with_maximum_number_of_levels = () =>
            {
                _skipList.Head.Levels.Count.ShouldEqual(SkipList.SkipListMaxLevel);
            };

            It should_set_the_back_link_on_the_head_node_to_null = () =>
            {
                _skipList.Head.Backward.ShouldBeNull();
            };

            It should_set_the_tail_node_to_null = () =>
            {
                _skipList.Tail.ShouldBeNull();
            };

            It should_set_the_length_to_0 = () =>
            {
                _skipList.Length.ShouldEqual(0);
            };

            It should_set_the_levels_to_1 = () =>
            {
                _skipList.Levels.ShouldEqual(1);
            };

            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_inserting_first_node_at_level_1
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
            };

            Because of = () =>
            {
                _newNode = _skipList.Insert(Score, Element);
            };

            It should_return_the_inserted_node = () =>
            {
                _newNode.ShouldNotBeNull();
                _newNode.Score.ShouldEqual(Score);
                _newNode.Element.ShouldEqual(Element);
                _newNode.Backward.ShouldBeNull();
                _newNode.Levels.Count.ShouldEqual(1);
            };

            It should_make_new_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_newNode);
            };

            It should_link_head_node_at_level_1_to_new_node = () =>
            {
                _skipList.Head.Levels[0].Forward.ShouldEqual(_newNode);
                _skipList.Head.Levels[0].Span.ShouldEqual(1);
            };

            It should_update_length_of_the_skip_list_to_1 = () =>
            {
                _skipList.Length.ShouldEqual(1);
            };

            It should_update_the_level_count_to_1 = () =>
            {
                _skipList.Levels.ShouldEqual(1);
            };

            const int Score = 1;
            const string Element = "one";

            static SkipListNode _newNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_inserting_first_node_at_level_1_and_last_node_at_level_2
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
            };

            Because of = () =>
            {
                _lastNode = _skipList.Insert(LastScore, LastElement);
            };

            It should_return_the_inserted_node = () =>
            {
                _lastNode.ShouldNotBeNull();
                _lastNode.Score.ShouldEqual(LastScore);
                _lastNode.Element.ShouldEqual(LastElement);
                _lastNode.Backward.ShouldEqual(_firstNode);
                _lastNode.Levels.Count.ShouldEqual(2);
            };

            It should_link_last_node_to_first_node = () =>
            {
                _lastNode.Backward.ShouldEqual(_firstNode);
            };

            It should_link_first_node_to_last_node = () =>
            {
                _firstNode.Levels[0].Forward.ShouldEqual(_lastNode);
                _firstNode.Levels[0].Span.ShouldEqual(1);
            };

            It should_make_last_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_lastNode);
            };

            It should_link_head_node_at_level_2_to_last_node = () =>
            {
                _skipList.Head.Levels[1].Forward.ShouldEqual(_lastNode);
                _skipList.Head.Levels[1].Span.ShouldEqual(2);
            };

            It should_update_length_to_2 = () =>
            {
                _skipList.Length.ShouldEqual(2);
            };

            It should_update_the_level_count_to_2 = () =>
            {
                _skipList.Levels.ShouldEqual(2);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const string FirstElement = "one";
            const string LastElement = "five";

            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_inserting_first_node_at_level_1_last_node_at_level_2_and_middle_node_at_level_3
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
                _lastNode = _skipList.Insert(LastScore, LastElement);
            };

            Because of = () =>
            {
                _middleNode = _skipList.Insert(MiddleScore, MiddleElement);
            };

            It should_return_the_inserted_node = () =>
            {
                _middleNode.ShouldNotBeNull();
                _middleNode.Score.ShouldEqual(MiddleScore);
                _middleNode.Element.ShouldEqual(MiddleElement);
                _middleNode.Backward.ShouldEqual(_firstNode);
                _middleNode.Levels.Count.ShouldEqual(3);
            };

            It should_link_first_node_to_middle_node = () =>
            {
                _firstNode.Levels[0].Forward.ShouldEqual(_middleNode);
                _firstNode.Levels[0].Span.ShouldEqual(1);
            };

            It should_link_middle_node_to_first_node = () =>
            {
                _middleNode.Backward.ShouldEqual(_firstNode);
            };

            It should_link_middle_node_to_last_node_at_levels_1_and_2 = () =>
            {
                _middleNode.Levels[0].Forward.ShouldEqual(_lastNode);
                _middleNode.Levels[0].Span.ShouldEqual(1);
                _middleNode.Levels[1].Forward.ShouldEqual(_lastNode);
                _middleNode.Levels[1].Span.ShouldEqual(1);
            };

            It should_link_last_node_to_middle_node = () =>
            {
                _lastNode.Backward.ShouldEqual(_middleNode);
            };

            It should_make_last_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_lastNode);
            };

            It should_link_head_node_at_levels_2_and_3_to_middle_node = () =>
            {
                _skipList.Head.Levels[1].Forward.ShouldEqual(_middleNode);
                _skipList.Head.Levels[1].Span.ShouldEqual(2);
                _skipList.Head.Levels[2].Forward.ShouldEqual(_middleNode);
                _skipList.Head.Levels[2].Span.ShouldEqual(2);
            };

            It should_update_length_of_the_skip_list_to_3 = () =>
            {
                _skipList.Length.ShouldEqual(3);
            };

            It should_update_the_level_count_to_3 = () =>
            {
                _skipList.Levels.ShouldEqual(3);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static SkipListNode _middleNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_deleting_the_only_node_in_the_skip_list
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);

                _skipList.Insert(Score, Element);
            };

            Because of = () =>
            {
                _deleteResult = _skipList.Delete(Score, Element);
            };

            It should_successfully_delete_node = () =>
            {
                _deleteResult.ShouldBeTrue();
            };

            It should_set_tail_to_null = () =>
            {
                _skipList.Tail.ShouldBeNull();
            };

            It should_unlink_head_node_at_level_1_from_deleted_node = () =>
            {
                _skipList.Head.Levels[0].Forward.ShouldBeNull();
                _skipList.Head.Levels[0].Span.ShouldEqual(0);
            };

            It should_update_length_of_the_skip_list_to_1 = () =>
            {
                _skipList.Length.ShouldEqual(0);
            };

            It should_update_the_level_count_to_1 = () =>
            {
                _skipList.Levels.ShouldEqual(1);
            };

            const int Score = 1;
            const string Element = "one";

            static bool _deleteResult;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_deleting_the_middle_node_from_skip_list
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
                _lastNode = _skipList.Insert(LastScore, LastElement);
                _middleNode = _skipList.Insert(MiddleScore, MiddleElement);
            };

            Because of = () =>
            {
                _deleteResult = _skipList.Delete(MiddleScore, MiddleElement);
            };

            It should_successfully_delete_node = () =>
            {
                _deleteResult.ShouldBeTrue();
            };

            It should_link_first_node_to_last_node = () =>
            {
                _firstNode.Levels[0].Forward.ShouldEqual(_lastNode);
                _firstNode.Levels[0].Span.ShouldEqual(1);
            };

            It should_link_last_node_to_first_node = () =>
            {
                _lastNode.Backward.ShouldEqual(_firstNode);
            };

            It should_make_last_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_lastNode);
            };

            It should_link_head_node_at_level_2_to_last_node = () =>
            {
                _skipList.Head.Levels[1].Forward.ShouldEqual(_lastNode);
                _skipList.Head.Levels[1].Span.ShouldEqual(2);
            };

            It should_unlink_head_node_at_level_3_from_deleted_middle_node = () =>
            {
                _skipList.Head.Levels[2].Forward.ShouldBeNull();
                _skipList.Head.Levels[2].Span.ShouldEqual(0);
            };

            It should_update_length_of_the_skip_list_to_2 = () =>
            {
                _skipList.Length.ShouldEqual(2);
            };

            It should_update_the_level_count_to_2 = () =>
            {
                _skipList.Levels.ShouldEqual(2);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            static bool _deleteResult;
            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static SkipListNode _middleNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_deleting_the_last_node_from_skip_list
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
                _lastNode = _skipList.Insert(LastScore, LastElement);
                _middleNode = _skipList.Insert(MiddleScore, MiddleElement);
            };

            Because of = () =>
            {
                _deleteResult = _skipList.Delete(LastScore, LastElement);
            };

            It should_successfully_delete_node = () =>
            {
                _deleteResult.ShouldBeTrue();
            };

            It should_unlink_middle_node_from_last_node_at_levels_1_and_2 = () =>
            {
                _middleNode.Levels[0].Forward.ShouldBeNull();
                _middleNode.Levels[0].Span.ShouldEqual(0);
                _middleNode.Levels[1].Forward.ShouldBeNull();
                _middleNode.Levels[1].Span.ShouldEqual(0);
            };

            It should_make_middle_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_middleNode);
            };

            It should_update_length_of_the_skip_list_to_2 = () =>
            {
                _skipList.Length.ShouldEqual(2);
            };

            It should_update_the_level_count_to_3 = () =>
            {
                _skipList.Levels.ShouldEqual(3);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            static bool _deleteResult;
            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static SkipListNode _middleNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_updating_the_only_node_in_the_skip_list
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.Setup(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);

                _skipList.Insert(Score, Element);
            };

            Because of = () =>
            {
                _updatedNode = _skipList.Update(Score, Element, NewScore);
            };

            It should_successfully_update_node = () =>
            {
                _updatedNode.ShouldNotBeNull();
                _updatedNode.Score.ShouldEqual(NewScore);
            };

            const int Score = 1;
            const int NewScore = 2;
            const string Element = "one";

            static SkipListNode _updatedNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_updating_the_middle_node_in_skip_list_with_larger_score_than_last_node
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3)
                    .Returns(3);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
                _lastNode = _skipList.Insert(LastScore, LastElement);
                _middleNode = _skipList.Insert(MiddleScore, MiddleElement);
            };

            Because of = () =>
            {
                _updatedNode = _skipList.Update(MiddleScore, MiddleElement, NewMiddleScore);
            };

            It should_successfully_update_node = () =>
            {
                _updatedNode.ShouldNotBeNull();
            };

            It should_link_first_node_to_last_node = () =>
            {
                _firstNode.Levels[0].Forward.ShouldEqual(_lastNode);
                _firstNode.Levels[0].Span.ShouldEqual(1);
            };

            It should_link_last_node_to_first_node = () =>
            {
                _lastNode.Backward.ShouldEqual(_firstNode);
            };

            It should_link_last_node_to_updated_node_at_levels_1_and_2 = () =>
            {
                _lastNode.Levels[0].Forward.ShouldEqual(_updatedNode);
                _lastNode.Levels[0].Span.ShouldEqual(1);
                _lastNode.Levels[1].Forward.ShouldEqual(_updatedNode);
                _lastNode.Levels[1].Span.ShouldEqual(1);
            };

            It should_link_updated_node_to_last_node = () =>
            {
                _updatedNode.Backward.ShouldEqual(_lastNode);
            };

            It should_make_updated_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_updatedNode);
            };

            It should_link_head_node_at_level_2_to_last_node = () =>
            {
                _skipList.Head.Levels[1].Forward.ShouldEqual(_lastNode);
                _skipList.Head.Levels[1].Span.ShouldEqual(2);
            };

            It should_unlink_head_node_at_level_3_from_updated_node = () =>
            {
                _skipList.Head.Levels[2].Forward.ShouldEqual(_updatedNode);
                _skipList.Head.Levels[2].Span.ShouldEqual(3);
            };

            It should_update_length_of_the_skip_list_to_3 = () =>
            {
                _skipList.Length.ShouldEqual(3);
            };

            It should_update_the_level_count_to_3 = () =>
            {
                _skipList.Levels.ShouldEqual(3);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const int NewMiddleScore = 6;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            static SkipListNode _updatedNode;
            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static SkipListNode _middleNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }

        [Subject(typeof(SkipList), "Skip List"), Tags("Positive Test")]
        public class When_deleting_a_range_of_nodes_by_score_from_skip_list
        {
            Establish context = () =>
            {
                _nodeLevelGeneratorMock.SetupSequence(mock => mock.Generate(Moq.It.IsAny<int>()))
                    .Returns(1)
                    .Returns(2)
                    .Returns(3);

                _skipList = new SkipList(_nodeLevelGeneratorMock.Object);
                _firstNode = _skipList.Insert(FirstScore, FirstElement);
                _lastNode = _skipList.Insert(LastScore, LastElement);
                _middleNode = _skipList.Insert(MiddleScore, MiddleElement);
            };

            Because of = () =>
            {
                _nodesDeleted = _skipList.DeleteRangeByScore(new SkipListRange
                {
                    Min = FirstScore,
                    Max = MiddleScore
                });
            };

            It should_successfully_delete_nodes_in_range = () =>
            {
                _nodesDeleted.ShouldEqual(2);
            };

            It should_make_last_node_the_tail_node = () =>
            {
                _skipList.Tail.ShouldEqual(_lastNode);
            };

            It should_link_head_node_at_levels_1_and_2_to_last_node = () =>
            {
                _skipList.Head.Levels[0].Forward.ShouldEqual(_lastNode);
                _skipList.Head.Levels[0].Span.ShouldEqual(1);
                _skipList.Head.Levels[1].Forward.ShouldEqual(_lastNode);
                _skipList.Head.Levels[1].Span.ShouldEqual(1);
            };

            It should_unlink_head_node_at_level_3_from_deleted_middle_node = () =>
            {
                _skipList.Head.Levels[2].Forward.ShouldBeNull();
                _skipList.Head.Levels[2].Span.ShouldEqual(0);
            };

            It should_update_length_of_the_skip_list_to_1 = () =>
            {
                _skipList.Length.ShouldEqual(1);
            };

            It should_update_the_level_count_to_2 = () =>
            {
                _skipList.Levels.ShouldEqual(2);
            };

            const int FirstScore = 1;
            const int LastScore = 5;
            const int MiddleScore = 3;
            const string FirstElement = "one";
            const string LastElement = "five";
            const string MiddleElement = "third";

            static long _nodesDeleted;
            static SkipListNode _firstNode;
            static SkipListNode _lastNode;
            static SkipListNode _middleNode;
            static Mock<ISkipListNodeLevelGenerator> _nodeLevelGeneratorMock = new Mock<ISkipListNodeLevelGenerator>();
            static SkipList _skipList;
        }
    }
}