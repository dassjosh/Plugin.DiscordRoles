﻿using Newtonsoft.Json;
using Oxide.Plugins;

namespace DiscordRolesPlugin.Data;

public class PluginData
{
    public Hash<string, PlayerData> PlayerData = new();

    [JsonIgnore] 
    public bool HasChanged { get; private set; }

    public PlayerData GetPlayerData(string playerId) => PlayerData[playerId];

    public PlayerData GetOrCreatePlayerData(string playerId)
    {
        PlayerData data = GetPlayerData(playerId);
        if (data == null)
        {
            data = new PlayerData(playerId);
            PlayerData[playerId] = data;
            OnDataChanged();
        }

        return data;
    }

    public void Cleanup(string playerId)
    {
        PlayerData.Remove(playerId);
        OnDataChanged();
    }

    public void OnDataChanged() => HasChanged = true;
    public void OnSaved() => HasChanged = false;
}