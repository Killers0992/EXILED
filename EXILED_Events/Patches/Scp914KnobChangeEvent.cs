using System;
using Harmony;
using Scp914;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
	public class Scp914KnobChangeEvent
	{
		public static bool Prefix(PlayerInteract __instance)
		{
			try
			{
				if (!__instance._playerInteractRateLimit.CanExecute(true) ||
				    __instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract ||
				    (Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.knob.position)))
					return false;
				Scp914Knob knobSetting = Scp914Machine.singleton.knobState;
				if (knobSetting + 1 > Scp914Machine.knobStateMax)
					knobSetting = Scp914Machine.knobStateMin;
				else
					knobSetting += 1;
				bool allow = true;
				Events.InvokeScp914KnobChange(__instance.gameObject, ref allow, ref knobSetting);

				if (allow)
				{
					Scp914Machine.singleton.ChangeKnobStatus();
					__instance.OnInteract();
				}

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"SCP914Knob Error: {e}");
				return true;
			}
		}
	}
}