namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;
    using System;

    class ScriptLoaderTests
    {
        [Subject(typeof(ResourceLoader), "Resource Loader"), Tags("Positive Test")]
        public class When_loading_script_from_assembly
        {
            Because of = () =>
            {
                _resourceBody = ResourceLoader.GetResourceAsync(ResourceName).Await();
            };

            It should_load_the_resource = () =>
            {
                _resourceBody.ShouldNotBeEmpty();
            };

            const string ResourceName = "request_rate_limiter.lua";

            static string _resourceBody;
        }

        [Subject(typeof(ResourceLoader), "Resource Loader"), Tags("Negative Test")]
        public class When_loading_non_existing_resource_from_assembly
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => ResourceLoader.GetResourceAsync(ResourceName).Await());
            };

            It should_throw_exception = () =>
            {
                _exception.ShouldBeOfExactType<InvalidOperationException>();
                _exception.Message.ShouldEqual($"The resource \"{ResourceName}\" does not exist.");
            };

            const string ResourceName = "does_not_exist.lua";

            static Exception _exception;
        }
    }
}