using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Orbit
{
    public class OrbitInputManager : RulesetInputManager<OrbitAction>
    {
        public OrbitInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum OrbitAction
    {
        [Description("Button 1")]
        Button1,
        [Description("Button 2")]
        Button2
    }
}
