# Congestion Control for AspNet Core

[![Build Status](https://github.com/sakopov/AspNetCore.CongestionControl/workflows/build/badge.svg)](https://github.com/sakopov/AspNetCore.CongestionControl)
[![NuGet Release](https://img.shields.io/nuget/v/AspNetCore.CongestionControl.svg)](https://www.nuget.org/packages/AspNetCore.CongestionControl)
[![Coverage Status](https://coveralls.io/repos/github/sakopov/AspNetCore.CongestionControl/badge.svg)](https://coveralls.io/github/sakopov/AspNetCore.CongestionControl)

*Congestion control middleware components for ASPNET Core.*

This library provides a set of middleware components designed to help maintain API availability and reliability via several commonly-used rate limiter algorithms.

> Note, the rate limiting methods presented here are based on work outlined in Stripe's Engineering blog post - [Scaling your API with rate limiters](https://stripe.com/blog/rate-limiters) - with a few trivial modifications.

## Installation

Install [AspNetCore.CongestionControl with NuGet](https://www.nuget.org/packages/AspNetCore.CongestionControl)

```shell
Install-Package AspNetCore.CongestionControl
```

## Basic Usage

The following example demostrates basic usage scenario which adds request rate limiting with default options.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl();

    ...

    services.AddMvc();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // Use the middleware
    app.UseCongestionControl();

    ...

    app.UseMvc();
}
```

## Overview

The following section outlines all supported congestion control mechanism and their underlying behavior.

### Request Rate Limiter

The Request Rate Limiter restricts each client to *N* requests per time interval using [Token Bucket](https://en.wikipedia.org/wiki/Token_bucket) algorithm. It can allow brief traffic spikes to burst above the capacity of the bucket.

There are 3 parameters that define the behavior of the Request Rate Limiter.

* **Interval** - the length of time unit (in seconds). The default value is *1 second*.
* **Average Rate** - how many requests per *interval* a client is allowed to perform. The default value is *100 requests*.
* **Bursting** - the bursting factor or how much bursting is allowed per *interval* which allows client to briefly go above the *averate rate* in a  traffic spikes. Note, *Bursting* x *Average Rate* = *Capacity* of the bucket. The default value is *5*.

#### Example

Use example below to add and configure Request Rate Limiter middleware.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        // Use one of the overrides to pass custom configurations
        options.AddRequestRateLimiter();
    });

    ...

    services.AddMvc();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // Use the middleware
    app.UseCongestionControl();

    ...

    app.UseMvc();
}
```

### Concurrent Request Limiter

The Concurrent Request Limiter restricts how many requests a client can have executing at the same time. This request limiter can help minimize resource contention on expensive endpoints.

There are 2 parameters that define the behavior of the Concurrent Request Limiter.

* **Capacity** - the number of requests a client can execute at the same time. The default value is *100 requests*.
* **Request TTL** - the amount of time (in seconds) a client request can take. The default value is *60 seconds*.

#### Example

Use example below to configure and add the Concurrent Request Limiter middleware.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        // Use one of the overrides to pass custom configurations
        options.AddConcurrentRequestLimiter();
    });

    ...

    services.AddMvc();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // Use the middleware
    app.UseCongestionControl();

    ...

    app.UseMvc();
}
```

### Client Identifiers

All middleware components require a way to uniquely identify API clients. A header-based client identification strategy is used by default where the client identifier is passed via `x-api-key` header.

Anonymous access is assumed if the client identifier cannot be found in the request. Note, if you choose to define a custom header, you can do so by specifying it in `AddHeaderBasedClientIdentifierProvider` method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        options.AddHeaderBasedClientIdentifierProvider();
    });

    ...

    services.AddMvc();
}
```

Alternatively, a query-based client identification strategy can be used to pass client identifier via `api_key` query string parameter. Note, if you choose to define a query string parameter, you can do so by specifying it in `AddQueryBasedClientIdentifierProvider` method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        options.AddQueryBasedClientIdentifierProvider();
    });

    ...

    services.AddMvc();
}
```

If you need to define a custom client identification strategy, you can do so my implementing `IClientIdentifierProvider` interface and using `AddClientIdentifierProvider` to pass it in.

```csharp
public class MyCustomClientIdProvider : IClientIdentifierProvider
{
    public Task<string> ExecuteAsync(HttpContext httpContext)
    {
        // Do something here to get client identifier
    }
}

public void ConfigureServices(IServiceCollection services)
{
    var customClientIdProvider = new MyCustomClientIdProvider();

    services.AddCongestionControl(options =>
    {
        options.AddClientIdentifierProvider(customClientIdProvider);
    });

    ...

    services.AddMvc();
}
```

Note, you can add multiple client providers if you choose to do so.

## HTTP Response Status Code

If a request is rate-limited by any of the middleware, the client will receive a response with HTTP status code **429 / Too Many Requests** (unless configured otherwise) and content type matching that of the original request. It's possible to provide a custom status code via `HttpStatusCode` property.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        // Use 503 / Service Unavailable
        options.HttpStatusCode = 503;
    });

    ...

    services.AddMvc();
}
```

## HTTP Response Formatting

If you need to customize the rate limit response, you can do so my implementing `IHttpResponseFormatter`. The example below shows HTTP response formatter which adds `X-RateLimit-Limit` and `X-RateLimit-Remaining` headers to the response.

> Note, it is not recommended to return rate limit and remaining number of requests if you're using both **Request Rate Limiter** and **Concurrent Request Limiter** simultaneously because each rate limiter throttles requests using different rate limits and the response can confuse the client.

```csharp
public class MyCustomHttpResponseFormatter : IHttpResponseFormatter
{
    public Task FormatAsync(HttpContext httpContext, RateLimitContext rateLimitContext)
    {
        httpContext.Response.ContentType = httpContext.Request.ContentType;
        httpContext.Response.StatusCode = (int)rateLimitContext.HttpStatusCode;
        httpContext.Response.Headers.Add("X-RateLimit-Limit", rateLimitContext.Limit.ToString());
        httpContext.Response.Headers.Add("X-RateLimit-Remaining", rateLimitContext.Remaining.ToString());

        return Task.CompletedTask;
    }
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        options.AddHttpResponseFormatter(new MyCustomHttpResponseFormatter());
    });

    ...

    services.AddMvc();
}
```

## Distributed Storage with Redis

By default, all middleware components will use in-memory storage. This might work for very basic usage and deployment scenarious. However, this type of storage is volatile due to high risk of data loss when the API recycles or restarts. For this, and other reasons, it's important to use high-performance, persistent storage alternative such as [Redis](https://redis.io/).

Use `AddRedisStorage` to configure the connection to your Redis instance.

> Note, since the library is using **StackExchange.Redis** to communicate with your Redis instance, you can refer to [this guide](https://stackexchange.github.io/StackExchange.Redis/Configuration#basic-configuration-strings) to read about various supported connection string formats.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCongestionControl(options =>
    {
        // Connects to local Redis
        options.AddRedisStorage("127.0.0.1:6379");
    });

    ...

    services.AddMvc();
}
```

## Considerations and Limitations

Rate limiters should be one of the first middleware components executing in your middleware pipeline.

### Client-Specific Rate Limits

Client-specific rate limits are currently not supported and there are currently no plans to introduce this functionality.
