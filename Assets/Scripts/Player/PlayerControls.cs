using UnityEngine;

[RequireComponent(typeof(MonsterCharacter))]
public class PlayerControls : MonoBehaviour
{
    [SerializeField] private string SoftcoreDeathScene = "Hideout";
    [SerializeField] private string HardcoreDeathScene = "Mainmenu";

    MonsterCharacter character;

    private void Awake()
    {
        Messaging.GUI.ActiveSlot.Invoke(-1);

        character = GetComponent<MonsterCharacter>();
        character.OnInputRequest = FillInputFrame;
        character.OnDeath.AddListener(() => { OnDeath(); });
        character.OnDamage.AddListener((_) => { Messaging.GUI.Painflash.Invoke(.25f); });

        character.OnWeaponChange.AddListener((i) =>
        {
            PlayerInfo.CurrentLocal.LastPlayerWeaponSwap = i;
            Messaging.GUI.ActiveSlot.Invoke(i);

            if (i >= 0 && i < character.weapons.Length && character.weapons[i] != null)
            {
                Messaging.GUI.LeftAbilityIcon.Invoke(character.weapons[i].LeftAbilitySprite);
                Messaging.GUI.RightAbilityIcon.Invoke(character.weapons[i].RightAbilitySprite);
            }
            else
            {
                Messaging.GUI.LeftAbilityIcon.Invoke(null);
                Messaging.GUI.RightAbilityIcon.Invoke(null);
            }
        });

        GetComponent<ThingController>().GridPosChanged.AddListener((p) => 
        {
            Messaging.Player.PlayerGridPosition.Invoke(p);

            int r = Room.GetRoomIndex(p);
            if (r != -1)
                Messaging.Player.PlayerEnterRoom.Invoke(r);
        });

        PlayerInfo.CurrentLocal.PlayerHasBeenInitialized = true;

        character.OnDeserialize.AddListener(() => 
        {
            Messaging.CameraControl.Spectator.Invoke(false);
            Messaging.CameraControl.RemoveShake.Invoke();
            Messaging.CameraControl.TargetPosition.Invoke(transform.position);
            Messaging.CameraControl.TargetRotation.Invoke(0);
            Messaging.CameraControl.SpeedMultiplier.Invoke(1f);
            Messaging.CameraControl.Teleport.Invoke();
        });

        Messaging.Player.EquipPlayerItems.AddListener(() => 
        {
            for (int w = 0; w < Mathf.Min(PlayerInfo.CurrentLocal.PlayerWeapons.Length, character.weapons.Length); w++)
            {
                if (PlayerInfo.CurrentLocal.PlayerWeapons == null)
                {
                    character.weapons[w] = null;
                    continue;
                }

                character.weapons[w] = PlayerInfo.CurrentLocal.PlayerWeapons[w];
            }

            for (int s = 0; s < Mathf.Min(PlayerInfo.CurrentLocal.PlayerSkills.Length, character.Skills.Length); s++)
            {
                if (PlayerInfo.CurrentLocal.PlayerSkills == null)
                {
                    character.Skills[s] = null;
                    continue;
                }

                character.Skills[s] = PlayerInfo.CurrentLocal.PlayerSkills[s];
            }

            SetActionBarIcons();
        });

        Messaging.Player.ForceHolster.AddListener(() => { character.UnEquip(); });
        Messaging.Player.DeActivateSkill.AddListener((i) => 
        {
            character.Skills[i]?.DeActivate(character, i);
        });

        Messaging.Player.ActivateAccessory.AddListener((i) => 
        {
            if (i >= 0 && i < PlayerInfo.CurrentLocal.PlayerAccessories.Length)
                PlayerInfo.CurrentLocal.PlayerAccessories[i]?.Activate(character);
        });

        Messaging.Player.DeActivateAccessory.AddListener((i) =>
        {
            if (i >= 0 && i < PlayerInfo.CurrentLocal.PlayerAccessories.Length)
                PlayerInfo.CurrentLocal.PlayerAccessories[i]?.DeActivate(character);
        });

        Messaging.Player.AddTalent.AddListener((talent) => 
        {
            if (PlayerInfo.CurrentLocal.TalentPoints <= 0)
            {
                Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH TALENT POINTS!", Color.red);
                return;
            }

            PlayerInfo.CurrentLocal.Talents.Add(talent);
            talent.Activate(character);
            PlayerInfo.CurrentLocal.TalentPoints--;
            Messaging.Player.TalentPoints.Invoke(PlayerInfo.CurrentLocal.TalentPoints);
        });

        Messaging.Player.RemoveTalent.AddListener((talent) =>
        {
            if (!PlayerInfo.CurrentLocal.Talents.Contains(talent))
            {
                Debug.LogError("PlayerControls: RemoveTalent: talent \"" + talent.name + "\" not found in PlayerInfo.CurrentLocal.Talents");
                return;
            }

            talent.DeActivate(character);
            PlayerInfo.CurrentLocal.Talents.Remove(talent);
            PlayerInfo.CurrentLocal.TalentPoints++;
            Messaging.Player.TalentPoints.Invoke(PlayerInfo.CurrentLocal.TalentPoints);
        });

        Messaging.Player.GetLocalPlayerStats = () => 
        {
            MonsterCharacter.ChargeRegenFrame frame = new MonsterCharacter.ChargeRegenFrame
            {
                RechargeAmount = character.ChargeRegenPerTick
            };

            character.OnChargeRegen.Invoke(frame);

            return new PlayerStats()
            {
                MaxHealth = character.MaxHitPoints / 1000,
                MaxCharge = character.MaxChargePoints / 1000,
                Armor = character.Armor,
                MovementSpeed = character.walkSpeed,
                ChargeRegen = (float)frame.RechargeAmount * 50f / 1000f,
                DamageDecay = (float)character.SoftDamageDecayPerTick * 50f / 1000f,
            };
        };

        Messaging.Player.Invulnerable.AddListener((b) => 
        {
            character.Invulnerable = b;
        });

        Messaging.GUI.OpenCanvas.Invoke("GameCanvas");
    }

    private void OnDestroy() =>
        Messaging.Player.GetLocalPlayerStats = () => { return null; };

    private void Start()
    {
        for (int a = 0; a < PlayerInfo.CurrentLocal.PlayerAccessories.Length; a++)
            Messaging.Player.ActivateAccessory.Invoke(a);

        foreach (Talent talent in PlayerInfo.CurrentLocal.Talents)
            talent.Activate(character);

        Messaging.Player.EquipPlayerItems.Invoke();
        character.SwapWeapon(PlayerInfo.CurrentLocal.LastPlayerWeaponSwap);

        //in case ammo amounts changed
        for (int a = 0; a < character.ammunition.Length; a++)
            character.ammunition[a] = character.maxAmmo[a];
    }

    private void OnDeath()
    {
        Messaging.Player.Health.Invoke(0, 1);
        //Messaging.Player.Charge.Invoke(0, 1);
        //Messaging.Player.SoftDamage.Invoke(0);

        Messaging.CameraControl.RemoveShake.Invoke();
        Messaging.System.SetTimeScale.Invoke(TimeScale.SuperSlowmo);
        Messaging.CameraControl.TargetRotation.Invoke(Random.value > .5f ? 90 : -90);

        //handle player info change instantly to make sure designator won't go ape shit
        SaveLoadSystem.SaveStash();
        PlayerInfo.CurrentLocal.PlayerHasBeenInitialized = false;
        if (Difficulty.PermanentDeath)
            SaveLoadSystem.DeletePlayerInfo();
        else
        {
            PlayerInfo.CurrentLocal.CurrentMap = SoftcoreDeathScene;
            SaveLoadSystem.SaveGame();
        }

        float deathTimer = 0f;
        GameObject deadwaker = new GameObject("Dead Waker");
        UpdateCallHook u = deadwaker.AddComponent<UpdateCallHook>();
        u.OnUpdateCall.AddListener(() => 
        {
            deathTimer += Time.unscaledDeltaTime;

            Messaging.GUI.Painflash.Invoke(1f);
            Messaging.GUI.Blackout.Invoke(deathTimer / 7f);

            if (deathTimer > 7f)
            {
                Destroy(deadwaker.gameObject);

                if (Difficulty.PermanentDeath)
                    UnityEngine.SceneManagement.SceneManager.LoadScene(HardcoreDeathScene);
                else
                    Messaging.System.ChangeLevel.Invoke(SoftcoreDeathScene, 0);
            }
        });

        Messaging.Player.Death.Invoke();
    }

    private void SetActionBarIcons()
    {
        for (int i = 0; i < character.weapons.Length; i++)
            if (character.weapons[i] != null)
                Messaging.GUI.SlotIcon.Invoke(i, character.weapons[i].InventoryIcon);
            else
                Messaging.GUI.SlotIcon.Invoke(i, null);

        for (int i = 0; i < character.Skills.Length; i++)
            if (character.Skills[i] != null)
                Messaging.GUI.SlotIcon.Invoke(i + 6, character.Skills[i].AbilityIcon);
            else
                Messaging.GUI.SlotIcon.Invoke(i + 6, null);
    }

    private void Update()
    {
        Messaging.Player.Health.Invoke(character.HitPoints, character.MaxHitPoints);
        Messaging.Player.Charge.Invoke(character.ChargePoints, character.MaxChargePoints);
        Messaging.Player.SoftDamage.Invoke(character.SoftDamage);
        Messaging.Player.Position.Invoke(transform.position);
        Messaging.Player.Velocity.Invoke(character.physicsBody.velocity);

        for (int i = 0; i < character.ammunition.Length; i++)
            Messaging.Player.Ammo.Invoke(i, character.ammunition[i], character.maxAmmo[i]);

        for (int i = 0; i < character.SkillCooldowns.Length; i++)
        {
            if (character.Skills[i] == null)
                Messaging.Player.SkillCooldown.Invoke(i, 0, 0, false);
            else
                Messaging.Player.SkillCooldown.Invoke(i, character.SkillCooldowns[i], character.Skills[i].ReferenceCooldown, character.Skills[i].ActiveInsteadOfCooldown);
        }
    }

    void FillInputFrame(ref MonsterCharacter.InputFrame frame)
    {
        if (HotkeyAssigment.Assigments[(int)Hotkey.Attack1].Pressed)
            frame.PrimaryAttackTrigger = true;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Attack2].Pressed)
            frame.SecondaryAttackTrigger = true;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Attack1].Continuous)
            frame.PrimaryAttackContinuous = true;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Attack2].Continuous)
            frame.SecondaryAttackContinuous = true;

        if (HotkeyAssigment.Assigments[(int)Hotkey.Up].Continuous)
            frame.up += 1f;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Down].Continuous)
            frame.up -= 1f;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Right].Continuous)
            frame.right += 1f;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Left].Continuous)
            frame.right -= 1f;

        if (HotkeyAssigment.Assigments[(int)Hotkey.Use].Pressed)
            frame.UseObject = true;

        if (HotkeyAssigment.Assigments[(int)Hotkey.Map].Pressed)
        {
            Messaging.GUI.PauseCurtain.Invoke();
            Messaging.GUI.OpenWindow.Invoke("UIMap");
            Messaging.System.SetTimeScale.Invoke(TimeScale.Paused);
            Messaging.GUI.ChangeCursor.Invoke(0);
        }

        //pretty hacky, should have proper cascading capture :'(
        if (Input.GetKeyDown(KeyCode.Escape) && !TimeScaler.Paused)
        {
            Messaging.GUI.PauseCurtain.Invoke();
            Messaging.GUI.OpenWindow.Invoke("Ingame Menu");
            Messaging.System.SetTimeScale.Invoke(TimeScale.Paused);
            Messaging.GUI.ChangeCursor.Invoke(0);
        }

        if (HotkeyAssigment.Assigments[(int)Hotkey.Weapon1].Pressed)
            frame.SwapWeapon = 0;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Weapon2].Pressed)
            frame.SwapWeapon = 1;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Weapon3].Pressed)
            frame.SwapWeapon = 2;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Weapon4].Pressed)
            frame.SwapWeapon = 3;
        if (HotkeyAssigment.Assigments[(int)Hotkey.Weapon5].Pressed)
            frame.SwapWeapon = 4;

        if (HotkeyAssigment.Assigments[(int)Hotkey.Skill1].Pressed)
            if (character.Skills.Length > 0 && character.Skills[0] != null)
                character.Skills[0].Activate(character, 0);
        if (HotkeyAssigment.Assigments[(int)Hotkey.Skill2].Pressed)
            if (character.Skills.Length > 1 && character.Skills[1] != null)
                character.Skills[1].Activate(character, 1);
        if (HotkeyAssigment.Assigments[(int)Hotkey.Skill3].Pressed)
            if (character.Skills.Length > 2 && character.Skills[2] != null)
                character.Skills[2].Activate(character, 2);
        if (HotkeyAssigment.Assigments[(int)Hotkey.Skill4].Pressed)
            if (character.Skills.Length > 3 && character.Skills[3] != null)
                character.Skills[3].Activate(character, 3);
        if (HotkeyAssigment.Assigments[(int)Hotkey.Skill5].Pressed)
            if (character.Skills.Length > 4 && character.Skills[4] != null)
                character.Skills[4].Activate(character, 4);

        frame.WantDirection = TowardsMouse;
    }

    public Quaternion TowardsMouse
    {
        get
        {
            Mouse.UpdateWorldPosition();
            Vector3 dif = (Mouse.WorldPosition - transform.position).DropZ();
            return Quaternion.Euler(0, 0, AxMath.SafeAtan2(dif.y, dif.x) * Mathf.Rad2Deg);
        }
    }
}
