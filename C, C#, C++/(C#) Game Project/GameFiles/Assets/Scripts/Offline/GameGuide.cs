using UnityEngine;
using TMPro;

public class GameGuide : MonoBehaviour
{
    void Start()
    {
        var tmp = GetComponent<TMP_Text>();
        if (tmp == null) return;

        tmp.text =
"<color=#FFD93D><size=15><b>GOAL</b></size></color>\n" +
"Collect the most <b>Gold</b> in <b>15 turns</b>. Highest gold wins.\n" +
"Tiebreaker: most combat items (Sword, Shield, Bow).\n\n" +

"<color=#FFD93D><size=15><b>TURN STRUCTURE</b></size></color>\n" +
"Each turn you plan <b>2 actions</b>: pick a <b>path color</b> (Red / Blue / Yellow) to move, then Yes or No to interact at the destination. Players alternate — when both have gone the turn counter advances.\n\n" +

"<color=#FFD93D><size=15><b>ABILITIES</b></size></color>\n" +
"Use an ability charge before confirming your moves:\n" +
"  <color=#A8E6CF><b>Bribe</b></color> — Steal 30-50% of opponent's Gold (or a random item if they're broke). Blocked by Fortify.\n" +
"  <color=#A8E6CF><b>Ambush</b></color> — Opponent skips their entire next turn. Blocked by Fortify.\n" +
"  <color=#A8E6CF><b>Teleport</b></color> — Warp instantly to any city on the map.\n" +
"  <color=#A8E6CF><b>Fortify</b></color> — Immune to Bribe and Ambush for <b>3 turns</b>.\n\n" +

"<color=#FFD93D><size=15><b>BUILDINGS</b></size></color>\n" +
"  <color=#FFB347><b>Market</b></color> — Spend <b>3 Gold</b> for a weapon, or <b>4 Gold</b> for a random ability scroll. 4-turn cooldown per player.\n\n" +
"  <color=#FF6B6B><b>Bandit Den</b></color> — Auto-triggers on landing. Steals one random item. <b>Wanderers immune.</b>\n\n" +
"  <color=#C3B1E1><b>Shrine</b></color> — Grants a free random ability charge, then disappears. Respawns at a new city after 5 turns.\n\n" +
"  <color=#98D8C8><b>Wandering Gambler</b></color> — Bet <b>5 Gold</b> on Odd or Even. Win = +5g, Lose = -5g. 4-turn cooldown per player.\n\n" +

"<color=#FFD93D><size=15><b>MAP OBJECTS</b></size></color>\n" +
"  <color=#F7DC6F><b>Chests</b></color> — Interact for a random reward: Gold, a weapon, or a rare ability scroll. A new chest spawns elsewhere after pickup.\n\n" +
"  <color=#E74C3C><b>Towers</b></color> — Requires <b>3 Sword + 3 Shield + 3 Bow</b> (Warriors need only 2 each). Success = consume weapons + <b>+10 Gold</b>. Failure = <b>-5 Gold</b>.\n\n" +

"<color=#FFD93D><size=15><b>CLASSES</b></size></color>\n" +
"  <color=#85C1E9><b>Merchant</b></color> — Starts with <b>3 Gold</b>. Ability scrolls at the Market cost 3g instead of 4.\n" +
"  <color=#85C1E9><b>Warrior</b></color> — Starts with weapons. Needs only <b>2 of each</b> to defeat a Tower.\n" +
"  <color=#85C1E9><b>Wanderer</b></color> — Immune to the Bandit Den.\n";
    }
}
