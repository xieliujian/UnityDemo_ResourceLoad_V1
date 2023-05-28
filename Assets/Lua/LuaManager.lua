
---@class LuaManager
LuaManager = class("LuaManager")

local M = LuaManager

function M:ctor()
    self.m_modules = {}
end

function M:Init()

    -- 加载lua配置文件
    self:LoadAllConfig()

    -- 初始化所有的模块
    self:InitModules()

    -- 进入游戏状态
    GameStateMgr.instance:ChangeState(GameStateType.Login)
end

function M:Update()
    self:UpdateModules()
end

function M:LoadAllConfig()
    require "Cfg.configmgr"
end

function M:UpdateModules()
    for _, module in ipairs(self.m_modules) do
        if module and module.Update then
            module:Update()
        end
    end
end

function M:ClearModules()
    for _, module in ipairs(self.m_modules) do
        if module and module.Clear then
            module:Clear()
        end
    end
end

function M:InitModules()
    -- 增加所有的模块
    self:AddModules()

    for _, module in ipairs(self.m_modules) do
        if module and module.Init then
            module:Init()
        end
    end
end

function M:AddModules()
    self.m_modules = {
        NetManager.instance,
        ClientModelMgr.instance,
        GameStateMgr.instance,
        UIManager.instance,
    }
end

LuaManager.instance = M:new()