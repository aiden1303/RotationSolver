using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class RDMCombo : CustomComboJob<RDMGauge>
{
    internal override uint JobID => 35;
    protected override bool CanHealSingleSpell => false;
    //����������û�дٽ�
    internal static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(1239);
    internal struct Actions
    {

        public static readonly BaseAction
            //��
            Jolt = new BaseAction(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�ش�
            Riposte = new BaseAction(7504)
            {
                OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
            },

            //������
            Verthunder = new BaseAction(7505)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�̱����
            CorpsAcorps = new BaseAction(7506)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
                }
            },

            //�༲��
            Veraero = new BaseAction(7507)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //ɢ��
            Scatter = new BaseAction(7509)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //������
            Verthunder2 = new BaseAction(16524u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ҷ�
            Veraero2 = new BaseAction(16525u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�����
            Verfire = new BaseAction(7510)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerfireReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ʯ
            Verstone = new BaseAction(7511)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerstoneReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //����ն
            Zwerchhau = new BaseAction(7512)
            {
                OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
            },

            //����
            Engagement = new BaseAction(16527),

            //�ɽ�
            Fleche = new BaseAction(7517),

            //����
            Redoublement = new BaseAction(7516)
            {
                OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
            },


            //�ٽ�
            Acceleration = new BaseAction(7518)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Acceleration },
            },

            //��Բն
            Moulinet = new BaseAction(7513),

            //������
            Vercure = new BaseAction(7514, true)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
            },

            //���ַ���
            ContreSixte = new BaseAction(7519u),

            //����
            Embolden = new BaseAction(7520),

            //����
            Manafication = new BaseAction(7521)
            {
                OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50,
            },

            //�ิ��
            Verraise = new BaseAction(7523, true)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide,
                OtherCheck = b => TargetHelper.DeathPeopleAll.Length > 0,
                BuffsProvide = new ushort[] { ObjectStatus.Raise },
            },

            //��ն
            Reprise = new BaseAction(16529),

            //����
            MagickBarrier = new BaseAction(25857);
    }

    private protected override bool EmergercyAbility( byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {

        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.ActionID == Actions.Zwerchhau.ActionID || nextGCD.ActionID == Actions.Redoublement.ActionID || nextGCD.ActionID == Actions.Moulinet.ActionID)
        {
            if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        }


        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //����Ҫ�ŵ�ħ������֮��
        if (JobGauge.ManaStacks == 3 || Service.ClientState.LocalPlayer.Level < 68)
        {
            if (Actions.Manafication.ShouldUseAction(out act)) return true;
        }
        //����������ʱ���ͷš�
        if (JobGauge.WhiteMana == 6 & JobGauge.BlackMana == 12)
        {
            if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.Manafication.ShouldUseAction(out act)) return true;
        }

        if (JobGauge.ManaStacks == 0)
        {
            //����ӽ��
            if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //�Ӹ�����
        if (GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;

        //�ٽ����˾��á� 
        if (Actions.Acceleration.ShouldUseAction(out act, mustUse: true)) return true;

        //�����ĸ���������
        if (Actions.ContreSixte.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Fleche.ShouldUseAction(out act)) return true;
        if (Actions.Engagement.ShouldUseAction(out act, Empty: IsBreaking)) return true;
        //if (Actions.CorpsAcorps.TryUseAction(level, out act)) return true;

        var target = Service.TargetManager.Target;
        if (Vector3.Distance(Service.ClientState.LocalPlayer.Position, target.Position) - target.HitboxRadius < 1)
        {
            if (Actions.CorpsAcorps.ShouldUseAction(out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //����Ѿ��ڱ����ˣ��Ǽ�����
        if (CanBreak(lastComboActionID, out act)) return true;

        //if (lastComboActionID == 0)
        //{
        //    if (Actions.Veraero2.ShouldUseAction(out act)) return true;
        //    act = Actions.Verthunder;
        //    return true;
        //}

        #region �������
        if (Actions.Verfire.ShouldUseAction(out act)) return true;
        if (Actions.Verstone.ShouldUseAction(out act)) return true;

        //���Կ�ɢ��
        if (Actions.Scatter.ShouldUseAction(out act)) return true;
        //ƽ��ħԪ
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Actions.Veraero2.ShouldUseAction(out act)) return true;
            if (Actions.Veraero.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.Verthunder2.ShouldUseAction(out act)) return true;
            if (Actions.Verthunder.ShouldUseAction(out act)) return true;
        }
        if (Actions.Jolt.ShouldUseAction(out act)) return true;
        #endregion
        //�����ƣ��Ӽ��̡�
        if (Actions.Vercure.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.Vercure.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.CorpsAcorps.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //����
        if (GeneralActions.Addle.ShouldUseAction(out act)) return true;
        return false;
    }
    internal static bool CanBreak(uint lastComboActionID, out BaseAction act)
    {
        byte level = Service.ClientState.LocalPlayer.Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Veraero2.ShouldUseAction(out act, mustUse: true)) return true;
            }
            if (Actions.Verthunder2.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //�����һ�δ��˳���ʥ���߳�˱���
        if (level >= 80 && (lastComboActionID == 7525 || lastComboActionID == 7526))
        {
            act = Actions.Jolt;
            return true;
        }

        //�����һ�δ��˽���
        if (level >= 90 && lastComboActionID == 16530)
        {
            act = Actions.Jolt;
            return true;
        }
        #endregion

        #region ��ս����

        if (lastComboActionID == Actions.Moulinet.ActionID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.Moulinet.ShouldUseAction(out act)) return true;
            if (Actions.Riposte.ShouldUseAction(out act)) return true;
        }
        if (Actions.Zwerchhau.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Redoublement.ShouldUseAction(out act, lastComboActionID)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = IsBreaking || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Actions.Embolden.IsCoolDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            foreach (var buff in Actions.Vercure.BuffsProvide)
            {
                if (BaseAction.HaveStatusSelfFromSelf(buff))
                {
                    return false;
                }
            }

            //���������ʱ��쵽�ˣ�������û�á�
            float emboldenRemain = Actions.Embolden.RecastTimeRemain;
            if (emboldenRemain < 30 && emboldenRemain > 1)
            {
                return false;
            }
        }

        #endregion

        #region ��������

        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Actions.Moulinet.ShouldUseAction(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Actions.Riposte.ShouldUseAction(out act)) return true;
        }

        #endregion
        return false;
    }

}