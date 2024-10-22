using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Handlers;
using DiscordRolesPlugin.Plugins;
using Oxide.Core;
using Oxide.Ext.Discord.Callbacks;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.Types;

namespace DiscordRolesPlugin.Sync;

public class ProcessUserCallback : BaseCallback
{
    private PlayerSyncRequest _request;
    private DiscordRoles _plugin;
    private Promise _promise;

    public readonly Action RunAction;
        
    public ProcessUserCallback()
    {
        RunAction = Run;
    }
        
    public static ProcessUserCallback Create(PlayerSyncRequest request, DiscordRoles plugin, Promise promise)
    {
        ProcessUserCallback callback = plugin.Pool.Get<ProcessUserCallback>();
        callback.Init(request, plugin, promise);
        return callback;
    }

    private void Init(PlayerSyncRequest request, DiscordRoles plugin, Promise promise)
    {
        _request = request;
        _plugin = plugin;
        _promise = promise;
    }
        
    protected override void HandleCallback()
    {
        try
        {
            if (_request.Member == null && !_request.IsLeaving)
            {
                _plugin.Logger.Debug("Skipping processing: {0}({1}) Failed to load Member with ID: {2}", _request.PlayerName, _request.Player.Id, _request.MemberId);
                return;
            }
            
            _plugin.Logger.Debug("Start processing: {0}({1}) Is Leaving: {2} Server Groups: {3} Discord Roles: {4}", _request.PlayerName, _request.Player.Id, _request.IsLeaving, _request.PlayerGroups, _request.PlayerRoles);

            List<BaseHandler> syncHandlers = _plugin.SyncHandlers;
            for (int index = 0; index < syncHandlers.Count; index++)
            {
                BaseHandler handler = syncHandlers[index];
                handler.Process(_request);
            }
            
            _plugin.HandleUserNick(_request);
            _plugin.Logger.Debug("Finish processing: {0}({1}) Is Leaving: {2} Server Groups: {3} Discord Roles: {4}", _request.PlayerName, _request.Player.Id, _request.IsLeaving, _request.PlayerGroups, _request.PlayerRoles);
        }
        catch (Exception ex)
        {
            _plugin.Logger.Exception("An error occured processing sync for {0}.\n{1}", _request.PlayerName, ex);
        }
        finally
        {
            _promise.Resolve();
        }
    }
}