namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;
    using SortedSet;

    class SkipListNodeLevelGeneratorTests
    {
        [Subject(typeof(SkipListNodeLevelGenerator), "Skip List Node Level Generator"), Tags("Positive Test")]
        public class When_generating_random_skip_list_level
        {
            Because of = () =>
            {
                _result = _generator.Generate(MaxLevel);
            };

            It should_generate_level_number_within_bounds = () =>
            {
                _result.ShouldBeGreaterThanOrEqualTo(1).ShouldBeLessThanOrEqualTo(MaxLevel);
            };

            const int MaxLevel = 10;

            static int _result;
            static SkipListNodeLevelGenerator _generator = new SkipListNodeLevelGenerator();
        }
    }
}