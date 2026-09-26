using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Orbit
{
    public class OrbitRuleset : Ruleset
    {
        public override string Description => "osu!orbit - Dual Planet Rhythm";
        public override string ShortName => "orbit";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            => new DrawableOrbitRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            => new OrbitBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            => new OrbitDifficultyCalculator(this, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
            => Array.Empty<Mod>();

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.CircleNotch };
    }

    public class DrawableOrbitRuleset : DrawableRuleset<OrbitHitObject>
    {
        public DrawableOrbitRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods) { }

        protected override Playfield CreatePlayfield() => new Playfield();
        protected override osu.Game.Rulesets.Objects.Drawables.DrawableHitObject<OrbitHitObject> CreateDrawableRepresentation(OrbitHitObject h) => null;
    }

    public class OrbitHitObject : HitObject { }

    public class OrbitBeatmapConverter : BeatmapConverter<OrbitHitObject>
    {
        public OrbitBeatmapConverter(IBeatmap beatmap, Ruleset ruleset) : base(beatmap, ruleset) { }
        public override bool CanConvert() => true;
        protected override IEnumerable<OrbitHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            yield return new OrbitHitObject { StartTime = original.StartTime };
        }
        protected override Beatmap<OrbitHitObject> CreateBeatmap() => new Beatmap<OrbitHitObject>();
    }

    public class OrbitDifficultyCalculator : DifficultyCalculator
    {
        public OrbitDifficultyCalculator(Ruleset ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap) { }
        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, System.ReadOnlySpan<Mod> mods) => new DifficultyAttributes();
    }
}

