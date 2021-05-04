// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryConcurrentRequestsManagerTests.cs">
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
    using System;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using Xunit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class InMemoryConcurrentRequestsManagerTests
    {
        [Fact(DisplayName = "Multiple Concurrent Requests Added Under Capacity")]
        public async void MultipleConcurrentRequestsAddedUnderCapacity()
        {
            // Given
            const string ClientId = "tester";

            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();
            var manager = new InMemoryConcurrentRequestsManager(configuration, loggerMock.Object);

            // When multiple requests are added
            var tasks = new List<Task<AddConcurrentRequestResult>>();
            tasks.Add(manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            tasks.Add(manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            var results = await Task.WhenAll(tasks);

            // Then the manager should allow first request
            results[0].IsAllowed.Should().BeTrue();
            results[0].Limit.Should().Be(configuration.Capacity);
            results[0].Remaining.Should().Be(configuration.Capacity - 1);

            // And the manager should allow second request
            results[1].IsAllowed.Should().BeTrue();
            results[1].Limit.Should().Be(configuration.Capacity);
            results[1].Remaining.Should().Be(configuration.Capacity - 2);
        }

        [Fact(DisplayName = "A Previously Added Request is Removed")]
        public async void APreviouslyAddedRequestIsRemoved()
        {
            // Given
            const string ClientId = "tester";
            const string RequestId = "123";

            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();
            var manager = new InMemoryConcurrentRequestsManager(configuration, loggerMock.Object);

            await manager.AddAsync(ClientId, RequestId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // When a request is removed
            var result = await manager.RemoveAsync(ClientId, RequestId);

            // Then the manager should remove the request
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "A Request is Added Over Capacity")]
        public async void ARequestIsAddedOverCapacity()
        {
            // Given
            const string ClientId = "tester";

            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();
            var manager = new InMemoryConcurrentRequestsManager(configuration, loggerMock.Object);

            for (var i = 0; i < configuration.Capacity; i++)
            {
                await manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }

            // When a request is added over capacity
            var result = await manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Then the manager should not allow the request
            result.IsAllowed.Should().BeFalse();
            result.Limit.Should().Be(configuration.Capacity);
            result.Remaining.Should().Be(0);
        }

        [Fact(DisplayName = "Removing a Request Which Was Never Added")]
        public async void RemovingARequestWhichWasNeverAdded()
        {
            // Given
            const string ClientId = "tester";

            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();
            var manager = new InMemoryConcurrentRequestsManager(configuration, loggerMock.Object);

            // When a request is removed
            var result = await manager.RemoveAsync(ClientId, Guid.NewGuid().ToString());

            // Then the manager should not remove anything
            result.Should().BeFalse();
        }
    }
}
