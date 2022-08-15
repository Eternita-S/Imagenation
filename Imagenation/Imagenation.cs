using Dalamud.Plugin;
using ECommons.Schedulers;
using System;

namespace Imagenation
{
    public class Imagenation : IDalamudPlugin
    {
        public string Name => "Imagenation";
        internal static Imagenation P;
        internal WindowSystem ws;
        internal Scene scene;
        internal List<DrawRequest> Requests = new();
        internal Http http;

        public Imagenation(DalamudPluginInterface pi)
        {
            P = this;
            ECommons.ECommons.Init(pi);
            new TickScheduler(delegate
            {
                scene = new();
                ws = new();
                ws.AddWindow(scene);
                Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
                Svc.PluginInterface.UiBuilder.DisableAutomaticUiHide = true;
                Svc.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
                Svc.PluginInterface.UiBuilder.DisableGposeUiHide = true;
                Svc.PluginInterface.UiBuilder.DisableUserUiHide = true;
                http = new();
            });
        }

        public void Dispose()
        {
            Safe(http.Dispose);
            Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
            ECommons.ECommons.Dispose();
            P = null;
        }
    }
}
