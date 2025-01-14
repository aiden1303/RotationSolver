﻿using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace RotationSolver.Basic;

public static class DataCenter
{

    private static List<NextAct> NextActs = new List<NextAct>();
    public static IBaseAction TimeLineAction { internal get; set; }
    internal static IBaseAction CommandNextAction
    {
        get
        {
            if (TimeLineAction != null) return TimeLineAction;

            var next = NextActs.FirstOrDefault();

            while (next != null && NextActs.Count > 0 && (next.deadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.act)))
            {
                NextActs.RemoveAt(0);
                next = NextActs.FirstOrDefault();
            }
            return next?.act;
        }
    }

    public static void AddOneTimelineAction(IBaseAction act, double time)
    {
        var index = NextActs.FindIndex(i => i.act.ID == act.ID);
        var newItem = new NextAct(act, DateTime.Now.AddSeconds(time));
        if (index < 0)
        {
            NextActs.Add(newItem);
        }
        else
        {
            NextActs[index] = newItem;
        }
        NextActs = NextActs.OrderBy(i => i.deadTime).ToList();
    }

    public static TargetHostileType RightNowTargetToHostileType
    {
        get
        {
            if (Service.Player == null) return 0;
            var id = Service.Player.ClassJob.Id;
            return GetTargetHostileType(Service.GetSheet<ClassJob>().GetRow(id));
        }
    }

    public static TargetHostileType GetTargetHostileType(ClassJob classJob)
    {
        if (Service.Config.TargetToHostileTypes.TryGetValue(classJob.RowId, out var type))
        {
            return (TargetHostileType)type;
        }

        return classJob.GetJobRole() == JobRole.Tank ? TargetHostileType.AllTargetsCanAttack : TargetHostileType.TargetsHaveTarget;
    }

    public static unsafe ActionID LastComboAction => (ActionID)ActionManager.Instance()->Combo.Action;
    public static unsafe float ComboTime => ActionManager.Instance()->Combo.Timer;
    internal static TargetingType TargetingType
    {
        get
        {
            if (Service.Config.TargetingTypes.Count == 0)
            {
                Service.Config.TargetingTypes.Add(TargetingType.Big);
                Service.Config.TargetingTypes.Add(TargetingType.Small);
                Service.Config.Save();
            }

            return Service.Config.TargetingTypes[Service.Config.TargetingIndex %= Service.Config.TargetingTypes.Count];
        }
    }

    public unsafe static bool IsMoving => AgentMap.Instance()->IsPlayerMoving > 0;

    public static unsafe ushort FateId
    {
        get
        {
            try
            {
                if (Service.Config.ChangeTargetForFate && (IntPtr)FateManager.Instance() != IntPtr.Zero
                    && (IntPtr)FateManager.Instance()->CurrentFate != IntPtr.Zero
                    && Service.Player.Level <= FateManager.Instance()->CurrentFate->MaxLevel)
                {
                    return FateManager.Instance()->CurrentFate->FateId;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex.StackTrace);
            }
            return 0;
        }
    }

    #region GCD
    public static float WeaponRemain { get; set; }

    public static float WeaponTotal { get; set; }

    public static float WeaponElapsed { get; set; }

    public static byte AbilityRemainCount { get; set; }

    public static float AbilityRemain { get; set; }

    public static float CastingTotal { get; set; }
    #endregion
    public static uint[] BluSlots { get; set; } = new uint[24];

    static DateTime _specialStateStartTime = DateTime.MinValue;
    public static double SpecialTimeLeft => Service.Config.SpecialDuration - (DateTime.Now - _specialStateStartTime).TotalSeconds;

    static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
    public static SpecialCommandType SpecialType =>
         SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;
    public static StateCommandType StateType { get; set; } = StateCommandType.Cancel;

    public static void SetSpecialType(SpecialCommandType specialType)
    {
        _specialType = specialType;
        _specialStateStartTime = specialType == SpecialCommandType.EndSpecial ? DateTime.MinValue : DateTime.Now;
    }

    public static bool InCombat { get; set; }

    public static float CombatTime { get; set; }

    public static IEnumerable<BattleChara> PartyMembers { get; set; } = new PlayerCharacter[0];

    public static IEnumerable<BattleChara> PartyTanks { get; set; } = new PlayerCharacter[0];

    public static IEnumerable<BattleChara> PartyHealers { get; set; } = new PlayerCharacter[0];

    public static IEnumerable<BattleChara> AllianceMembers { get; set; } = new PlayerCharacter[0];

    public static IEnumerable<BattleChara> AllianceTanks { get; set; } = new PlayerCharacter[0];

    public static ObjectListDelay<BattleChara> DeathPeopleAll { get; } = new(
    () => (Service.Config.DeathDelayMin, Service.Config.DeathDelayMax));

    public static ObjectListDelay<BattleChara> DeathPeopleParty { get; } = new(
        () => (Service.Config.DeathDelayMin, Service.Config.DeathDelayMax));

    public static ObjectListDelay<BattleChara> WeakenPeople { get; } = new(
        () => (Service.Config.WeakenDelayMin, Service.Config.WeakenDelayMax));

    public static ObjectListDelay<BattleChara> DyingPeople { get; } = new(
        () => (Service.Config.WeakenDelayMin, Service.Config.WeakenDelayMax));

    public static ObjectListDelay<BattleChara> HostileTargets { get; } = new ObjectListDelay<BattleChara>(
    () => (Service.Config.HostileDelayMin, Service.Config.HostileDelayMax));

    public static IEnumerable<BattleChara> AllHostileTargets { get; set; } = new BattleChara[0];

    public static IEnumerable<BattleChara> TarOnMeTargets { get; set; } = new BattleChara[0];

    public static ObjectListDelay<BattleChara> CanInterruptTargets { get; } = new ObjectListDelay<BattleChara>(
        () => (Service.Config.InterruptDelayMin, Service.Config.InterruptDelayMax));

    public static IEnumerable<GameObject> AllTargets { get; set; }

    public static uint[] TreasureCharas { get; set; } = new uint[0];
    public static bool HasHostilesInRange { get; set; }

    public static bool IsHostileCastingAOE { get; set; }

    public static bool IsHostileCastingToTank { get; set; }

    public static bool HasPet { get; set; }

    public static unsafe bool HasCompanion => (IntPtr)Service.RawPlayer == IntPtr.Zero ? false :
        (IntPtr)CharacterManager.Instance()->LookupBuddyByOwnerObject(Service.RawPlayer) != IntPtr.Zero;

    #region HP
    public static IEnumerable<float> PartyMembersHP { get; set; }
    public static float PartyMembersMinHP { get; set; }
    public static float PartyMembersAverHP { get; set; }
    public static float PartyMembersDifferHP { get; set; }

    public static bool HPNotFull { get; set; }

    public static bool CanHealAreaAbility { get; set; }

    public static bool CanHealAreaSpell { get; set; }

    public static bool CanHealSingleAbility { get; set; }

    public static bool CanHealSingleSpell { get; set; }
    #endregion
    public static Queue<MacroItem> Macros { get; } = new Queue<MacroItem>();

    #region Action Record
    const int QUEUECAPACITY = 32;
    private static Queue<ActionRec> _actions = new Queue<ActionRec>(QUEUECAPACITY);

    public static ActionRec[] RecordActions => _actions.Reverse().ToArray();
    private static DateTime _timeLastActionUsed = DateTime.Now;
    public static TimeSpan TimeSinceLastAction => DateTime.Now - _timeLastActionUsed;

    internal static ActionID LastAction { get; private set; } = 0;

    internal static ActionID LastGCD { get; private set; } = 0;

    internal static ActionID LastAbility { get; private set; } = 0;
    public static void AddActionRec(Action act)
    {
        var id = (ActionID)act.RowId;

        //Record
        switch (act.GetActionType())
        {
            case ActionCate.Spell: //魔法
            case ActionCate.WeaponSkill: //战技
                LastGCD = id;
                break;
            case ActionCate.Ability: //能力
                LastAbility = id;
                break;
            default:
                return;
        }
        LastAction = id;

        if (_actions.Count >= QUEUECAPACITY)
        {
            _actions.Dequeue();
        }
        _timeLastActionUsed = DateTime.Now;
        _actions.Enqueue(new ActionRec(_timeLastActionUsed, act));
    }
    #endregion

}
