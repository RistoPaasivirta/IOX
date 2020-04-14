using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PlayerStatsText : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.Player.LoadoutRefresh.AddListener(() => 
        {
            Refresh();
        });
    }

    private void OnEnable() =>
        Refresh();

    private void Refresh()
    {
        PlayerStats stats = Messaging.Player.GetLocalPlayerStats();

        if (stats == null)
            return;

        string s = 
        
        "MAX HEALTH:" + Environment.NewLine +
        stats.MaxHealth + Environment.NewLine +
        Environment.NewLine +
        
        "MAX CHARGE:" + Environment.NewLine +
        stats.MaxCharge + Environment.NewLine +
        Environment.NewLine +

        "MOVEMENT SPEED:" + Environment.NewLine +
        stats.MovementSpeed.ToString("F2") + Environment.NewLine +
        Environment.NewLine +

        "ARMOR:" + Environment.NewLine +
        stats.Armor + Environment.NewLine +
        Environment.NewLine +

        "CHARGE REGENERATION:" + Environment.NewLine +
        stats.ChargeRegen.ToString("F1") + Environment.NewLine +
        Environment.NewLine +

        "ACCUMULATED DAMAGE DECAY:" + Environment.NewLine +
        stats.DamageDecay.ToString("F1") + Environment.NewLine;

        text.text = s;
    }
}

public class PlayerStats
{
    public int MaxHealth;
    public int MaxCharge;
    public float MovementSpeed;
    public int Armor;
    public float DamageDecay;
    public float ChargeRegen;
}