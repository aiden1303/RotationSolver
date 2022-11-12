using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SGECombos.SGECombo_Default;

namespace XIVAutoAttack.Combos.Healer.SGECombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/SGECombos/SGECombo_Default.cs")]
internal sealed class SGECombo_Default : SGECombo_Base<CommandType>
{
    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"GCD: {Prognosis}\n                     ����: {Holos}, {Ixochole}, {Physis2}, {Physis}"},
        {DescType.��������, $"GCD: {Diagnosis}\n                     ����: {Druochole}"},
        {DescType.��Χ����, $"{Panhaima}, {Kerachole}, {Prognosis}"},
        {DescType.�������, $"GCD: {Diagnosis}\n                     ����: {Haima}, {Taurochole}"},
        {DescType.�ƶ�����, $"{Icarus}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ"},
    };
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //�¸�������
        if (nextGCD.IsAnySameAction(false, Pneuma, EukrasianDiagnosis,
            EukrasianPrognosis, Diagnosis, Prognosis))
        {
            //�
            if (Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //���
            if (Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall == 0)
        {
            //��Ѫ
            if (Haima.ShouldUse(out act)) return true;
        }

        //��ţ��֭
        if (Taurochole.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.HaveStatus(
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����Ѫ
        if (JobGauge.Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
        {
            if (Panhaima.ShouldUse(out act)) return true;
        }

        //�����֭
        if (Kerachole.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //Ԥ��
        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.HaveStatus(
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Icarus.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //�Ĺ�
        if (Kardia.ShouldUse(out act)) return true;

        //����
        if (JobGauge.Addersgall == 0 && Rhizomata.ShouldUse(out act)) return true;

        //����
        if (Soteria.ShouldUse(out act)) return true;

        //����
        if (Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //����
        if (JobGauge.Addersting == 3 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;
        //����
        if (Phlegma3.ShouldUse(out act, mustUse: Phlegma3.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: Phlegma2.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: Phlegma.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;

        //ʧ��
        if (Dyskrasia.ShouldUse(out act)) return true;

        if (EukrasianDosis.ShouldUse(out var enAct))
        {
            //����Dot
            if (Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(out act)) return true;
            if (DefenseSingleGCD(out act)) return true;
        }

        //עҩ
        if (Dosis.ShouldUse(out act)) return true;

        //����
        if (Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (JobGauge.Addersting > 0 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //��ս��Tˢ�����ζ���
        if (!InCombat)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && EukrasianDiagnosis.Target == tank.First() && EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().HaveStatus(
                    StatusID.EukrasianDiagnosis,
                    StatusID.EukrasianPrognosis,
                    StatusID.Galvanize
                )) return false;

                //����
                if (Eukrasia.ShouldUse(out act)) return true;

                act = EukrasianDiagnosis;
                return true;
            }
            if (Eukrasia.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��ţ��֭
        if (Taurochole.ShouldUse(out act)) return true;

        //������֭
        if (Druochole.ShouldUse(out act)) return true;

        //����Դ����ʱ���뷶Χ���ƻ���ѹ��
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (JobGauge.Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //������
            if (Holos.ShouldUse(out act)) return true;

            //����2
            if (Physis2.ShouldUse(out act)) return true;
            //����
            if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

            //����Ѫ
            if (Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.HaveStatus(
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
            {
                if (Diagnosis.ShouldUse(out act)) return true;
            }

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        //���
        if (Diagnosis.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.55f)
        {
            //�����Ϣ
            if (Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianPrognosis.Target.HaveStatus(
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            ))
            {
                if (Prognosis.ShouldUse(out act)) return true;
            }

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        //Ԥ��
        if (Prognosis.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //�����֭
        if (Kerachole.ShouldUse(out act)) return true;

        //����2
        if (Physis2.ShouldUse(out act)) return true;
        //����
        if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //������֭
        if (Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}