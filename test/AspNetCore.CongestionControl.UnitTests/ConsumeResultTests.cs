namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;

    class ConsumeResultTests
    {
        [Subject(typeof(ConsumeResult), "Consume Result"), Tags("Positive Test")]
        public class When_creating_new_instance
        {
            Because of = () =>
            {
                _result = new ConsumeResult(true, 10, 20);
            };

            It should_return_the_expected_result = () =>
            {
                _result.IsAllowed.ShouldBeTrue();
                _result.Limit.ShouldEqual(20);
                _result.Remaining.ShouldEqual(10);
            };

            static ConsumeResult _result;
        }
    }
}