
-- 逻辑
require("LuaInclude")
require("LuaManager")

main = {}

function main.CSStart()
    LuaManager.instance:Init()
end

function main.CSUpdate()
    LuaManager.instance:Update()
end

function CSMsgEvent(msgid, bytearray)
    NetManager.instance:Dispatcher(msgid, bytearray)
end

return main





