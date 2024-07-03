using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Utility;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imagenation
{
    internal class Scene : Window
    {
        bool InCombat = false;
        public Scene() : base("Imagenation scene", ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground, true)
        {
            this.RespectCloseHotkey = false;
            this.IsOpen = true;
        }

        public override void PreDraw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            this.Size = ImGuiHelpers.MainViewport.Size;
            this.Position = Vector2.Zero;
        }

        public override void Draw()
        {
            if (InCombat)
            {
                if (!Svc.Condition[ConditionFlag.InCombat])
                {
                    InCombat = false;
                    PluginLog.Debug("Combat exited");
                    P.Requests.RemoveAll(x => x.ResetOnWipe);
                }
            }
            else
            {
                if (Svc.Condition[ConditionFlag.InCombat])
                {
                    InCombat = true;
                    PluginLog.Debug("Combat started");
                }
            }
            foreach(var x in P.Requests)
            {
                if(ThreadLoadImageHandler.TryGetTextureWrap(x.Path, out var tw))
                {
                    var sw = (float)(ImGuiHelpers.MainViewport.Size.X - (float)tw.Width) / 100f;
                    var sh = (float)(ImGuiHelpers.MainViewport.Size.Y - (float)tw.Height) / 100f;
                    ImGui.SetCursorPos(new(sw * x.x, sh * x.y));
                    ImGui.Image(tw.ImGuiHandle, new(tw.Width, tw.Height));
                }
            }
            P.Requests.RemoveAll(x => Environment.TickCount64 > x.DestroyAt);
        }

        public override void PostDraw()
        {
            ImGui.PopStyleVar();
        }
    }
}
