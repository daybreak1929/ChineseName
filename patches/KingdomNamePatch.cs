using HarmonyLib;
using NeoModLoader.General.Event.Handlers;
using NeoModLoader.General.Event.Listeners;

namespace Chinese_Name;

public class KingdomNamePatch : IPatch
{
    class RenameKingdom : KingdomSetupHandler
    {
        public override void Handle(Kingdom pKingdom, bool pCiv)
        {
            if (!pCiv) return;
            if (!string.IsNullOrEmpty(pKingdom.data.name)) return;

            string name_generator_id = "human_kingdom";
            if (pKingdom.race == null)
            {
                ModClass.LogWarning($"No found race for kingdom {pKingdom.id} at {pKingdom.location}, use default name generator(human).");
            }
            else
            {
                name_generator_id = pKingdom.race.name_template_kingdom;
            }

            var asset = CN_NameGeneratorLibrary.Instance.get(name_generator_id);
            if (asset == null) return;
            var template = asset.GetRandomTemplate();
            
            var para = template.GetParametersToFill();
            
            
            
            pKingdom.data.name = template.GenerateName(para);
        }
    }
    public void Initialize()
    {
        KingdomSetupListener.RegisterHandler(new RenameKingdom());
        new Harmony(nameof(set_kingdom_motto)).Patch(AccessTools.Method(typeof(Kingdom), nameof(Kingdom.getMotto)),
            prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(set_kingdom_motto))));
    }

    private static bool set_kingdom_motto(Kingdom __instance)
    {
        if (!string.IsNullOrEmpty(__instance.data.motto)) return true;
        var generator = CN_NameGeneratorLibrary.Instance.get("kingdom_mottos");
        if (generator == null) return true;
        var template = generator.GetRandomTemplate();
        var para = template.GetParametersToFill();
        __instance.data.motto = template.GenerateName(para);
        return true;
    }
}