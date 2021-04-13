namespace AspNetCore.CongestionControl.UnitTests
{
    using System.Collections.Generic;
    using Machine.Specifications;

    class HttpContextItemsDictionaryExtensionsTests
    {
        [Subject(typeof(HttpContextItemsDictionaryExtensions), "HttpContext Items Dictionary Extensions"), Tags("Positive Test")]
        public class When_adding_client_id
        {
            Because of = () =>
            {
                items.AddClientId("client-id");
            };

            It the_extension_method_should_return_the_expected_client_id = () =>
            {
                items.GetClientId().ShouldEqual("client-id");
            };

            static IDictionary<object, object> items = new Dictionary<object, object>();
        }

        [Subject(typeof(HttpContextItemsDictionaryExtensions), "HttpContext Items Dictionary Extensions"), Tags("Negative Test")]
        public class When_client_id_does_not_exist
        {
            Because of = () =>
            {
                clientId = items.GetClientId();
            };

            It the_extension_method_should_return_null = () =>
            {
                clientId.ShouldBeNull();
            };

            static string clientId;

            static IDictionary<object, object> items = new Dictionary<object, object>();
        }
    }
}
