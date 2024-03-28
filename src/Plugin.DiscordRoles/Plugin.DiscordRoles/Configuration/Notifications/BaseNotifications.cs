using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Enums;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Configuration.Notifications;

public abstract class BaseNotifications
{
    [JsonProperty(PropertyName = "Send Message When Added")]
    public bool SendMessageOnAdd { get; set; }

    [JsonProperty(PropertyName = "Send Message When Removed")]
    public bool SendMessageOnRemove { get; set; }
        
    [JsonProperty(PropertyName = "Message Localization Key")]
    public string LocalizationKey { get; set; }

    [JsonIgnore]
    public string GroupAddedKey { get; set; }
        
    [JsonIgnore]
    public string GroupRemoveKey { get; set; }
        
    [JsonIgnore]
    public string RoleAddedKey { get; set; }
        
    [JsonIgnore]
    public string RoleRemoveKey { get; set; }
        
    [JsonIgnore]
    public TemplateKey GroupAddedTemplate { get; set; }
        
    [JsonIgnore]
    public TemplateKey GroupRemoveTemplate { get; set; }
        
    [JsonIgnore]
    public TemplateKey RoleAddedTemplate { get; set; }
        
    [JsonIgnore]
    public TemplateKey RoleRemoveTemplate { get; set; }

    protected BaseNotifications() { }
        
    protected BaseNotifications(BaseNotifications settings)
    {
        SendMessageOnAdd = settings?.SendMessageOnAdd ?? false;
        SendMessageOnRemove = settings?.SendMessageOnRemove ?? false;
        LocalizationKey = settings?.LocalizationKey ?? "Default";
    }
        
    public bool CanSendNotification(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.GroupAdded:
            case NotificationType.RoleAdded:
                return SendMessageOnAdd;
                    
            case NotificationType.GroupRemoved:
            case NotificationType.RoleRemoved:
                return SendMessageOnRemove;
 
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public string GetLocalizationKey(NotificationType type)
    {
        return type switch
        {
            NotificationType.GroupAdded => GroupAddedKey,
            NotificationType.GroupRemoved => GroupRemoveKey,
            NotificationType.RoleAdded => RoleAddedKey,
            NotificationType.RoleRemoved => RoleRemoveKey,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
        
    public TemplateKey GetLocalizationTemplate(NotificationType type)
    {
        return type switch
        {
            NotificationType.GroupAdded => GroupAddedTemplate,
            NotificationType.GroupRemoved => GroupRemoveTemplate,
            NotificationType.RoleAdded => RoleAddedTemplate,
            NotificationType.RoleRemoved => RoleRemoveTemplate,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public abstract void AddLocalizations(Dictionary<string, string> loc);
}