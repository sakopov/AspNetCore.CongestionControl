local key = @key
local capacity = tonumber(@capacity)
local timestamp = tonumber(@timestamp)
local requestId = @requestId

local count = redis.call("zcard", key)
local allowed = count < capacity

if allowed then
  redis.call("zadd", key, timestamp, requestId)
end

return { allowed, count }