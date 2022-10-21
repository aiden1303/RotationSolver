﻿using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Actions
{
    internal static class ObjectInfomation
    {
        private unsafe static BNpcBase GetObjectNPC(this GameObject obj)
        {
            if (obj == null) return null;
            var ptr = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)obj.Address;
            return Service.DataManager.GetExcelSheet<BNpcBase>().GetRow(ptr->GetNpcID());
        }

        internal static bool HasLocationSide(this GameObject obj)
        {
            if (obj == null) return false;
            return !(obj.GetObjectNPC()?.Unknown10 ?? false);
        }

        internal static bool IsBoss(this BattleChara obj)
        {
            if (obj == null) return false;
            return obj.MaxHp >= TargetFilter.GetHealthFromMulty(6.5f);
            //return !obj.GetObjectNPC().IsTargetLine;
        }

        internal static float GetHealthRatio(this BattleChara b)
        {
            if (b == null) return 0;
            return (float)b.CurrentHp / b.MaxHp;
        }

        /// <summary>
        /// 用于倾泻所有资源来收尾
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static bool IsDying(this BattleChara b)
        {
            if (b == null) return false;
            return b.CurrentHp <= TargetFilter.GetHealthFromMulty(1);
        }

        internal static EnemyLocation FindEnemyLocation(this GameObject enemy)
        {
            Vector3 pPosition = enemy.Position;
            float rotation = enemy.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            Vector3 dir = Service.ClientState.LocalPlayer.Position - pPosition;
            Vector2 dirVec = new Vector2(dir.Z, dir.X);

            double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

            if (angle < Math.PI / 4) return EnemyLocation.Front;
            else if (angle > Math.PI * 3 / 4) return EnemyLocation.Back;
            return EnemyLocation.Side;
        }
    }
}