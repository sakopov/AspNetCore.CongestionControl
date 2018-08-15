local tokens_key = @tokens_key
local timestamp_key = @timestamp_key
local refill_rate = tonumber(@refill_rate)
local refill_time = tonumber(@refill_time)
local capacity = tonumber(@capacity)
local now = tonumber(@timestamp)
local requested = tonumber(@requested)

local ttl = math.floor(refill_time * 2)

local last_tokens = tonumber(redis.call("get", tokens_key))
if last_tokens == nil then
  last_tokens = capacity
end

local last_refreshed = tonumber(redis.call("get", timestamp_key))
if last_refreshed == nil then
  last_refreshed = 0
end

local seconds_since_last_update = math.max(0, now - last_refreshed)
local refills_since_last_update = math.floor(seconds_since_last_update / refill_time)
local available_tokens_since_last_update = math.min(capacity, last_tokens + (refills_since_last_update * refill_rate))
local allowed = available_tokens_since_last_update >= requested
local new_tokens = available_tokens_since_last_update

if allowed then
  new_tokens = available_tokens_since_last_update - requested
end

local new_update_timestamp = last_refreshed + (refills_since_last_update * refill_time)

redis.call("setex", tokens_key, ttl, new_tokens)
redis.call("setex", timestamp_key, ttl, new_update_timestamp)

return { allowed, new_tokens }