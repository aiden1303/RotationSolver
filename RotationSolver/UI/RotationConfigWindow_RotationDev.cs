﻿using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System.Numerics;

namespace RotationSolver.UI;
internal partial class RotationConfigWindow
{
    private void DrawRotationDevTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_RotationDev_Description);

        if (ImGui.Button("Load Rotations"))
        {
            RotationUpdater.GetAllCustomRotations();
        }

        ImGui.SameLine();
        if (ImGui.Button("Dev Wiki"))
        {
            Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
        }

        ImGui.SameLine();

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Param_InDebug,
            ref Service.Config.InDebug);

        if (ImGui.BeginChild("Third-party Libs", new Vector2(0f, -1f), true))
        {
            if (ImGui.Button("AddOne"))
            {
                Service.Config.OtherLibs = Service.Config.OtherLibs.Append(string.Empty).ToArray();
            }
            ImGui.SameLine();
            ImGui.Text("Third-party Rotation Libraries");

            int removeIndex = -1;
            for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
            {
                if (ImGui.InputText($"##OtherLib{i}", ref Service.Config.OtherLibs[i], 1024))
                {
                    Service.Config.Save();
                }
                ImGui.SameLine();
                if (ImGui.Button($"X##Remove{i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = Service.Config.OtherLibs.ToList();
                list.RemoveAt(removeIndex);
                Service.Config.OtherLibs = list.ToArray();
                Service.Config.Save();
            }

            ImGui.EndChild();

        }
    }
}
