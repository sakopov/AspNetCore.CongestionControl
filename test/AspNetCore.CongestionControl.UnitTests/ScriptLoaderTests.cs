using System;
using Machine.Specifications;

namespace AspNetCore.CongestionControl.UnitTests
{
    class ScriptLoaderTests
    {
        [Subject(typeof(ScriptLoader), "Script Loader"), Tags("Positive Test")]
        public class When_loading_script_from_assembly
        {
            Because of = () =>
            {
                _scriptBody = ScriptLoader.GetScriptAsync(ScriptName).Await();
            };

            It should_load_the_script = () =>
            {
                _scriptBody.ShouldNotBeEmpty();
            };

            const string ScriptName = "request_rate_limiter.lua";

            static string _scriptBody;
        }

        [Subject(typeof(ScriptLoader), "Script Loader"), Tags("Negative Test")]
        public class When_loading_non_existing_script_from_assembly
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => ScriptLoader.GetScriptAsync(ScriptName).Await());
            };

            It should_throw_exception = () =>
            {
                _exception.ShouldBeOfExactType<InvalidOperationException>();
                _exception.Message.ShouldEqual($"The script \"{ScriptName}\" does not exist.");
            };

            const string ScriptName = "does_not_exist.lua";

            static Exception _exception;
        }
    }
}