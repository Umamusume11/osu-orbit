using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Objects;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Orbit
{
    // 1. MAIN RULESET ENTRY POINT
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

        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.CircleNotch
        };
    }

    // 2. CORE PLAYFIELD & ORBIT ENGINE
    public class OrbitPlayfield : Playfield
    {
        private readonly Circle firePlanet;
        private readonly Circle waterPlanet;
        
        private const float orbit_radius = 80f;
        private Vector2 centerPosition = new Vector2(512, 384);
        private bool isWaterPivot = true;

        public OrbitPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                firePlanet = new Circle { Size = new Vector2(25), Origin = Anchor.Centre, Colour = Color4.Red },
                waterPlanet = new Circle { Size = new Vector2(25), Origin = Anchor.Centre, Colour = Color4.DeepSkyBlue }
            };
        }

        public void SwitchPlanetPivot()
        {
            isWaterPivot = !isWaterPivot;
            centerPosition = isWaterPivot ? waterPlanet.Position : firePlanet.Position;
        }

        protected override void Update()
        {
            base.Update();
            double currentTime = Time.Current;
            float speedMultiplier = 0.2f;
            float angle = (float)(currentTime * speedMultiplier % 360);
            float radians = angle * (float)Math.PI / 180;

            if (isWaterPivot)
            {
                waterPlanet.Position = centerPosition;
                firePlanet.Position = centerPosition + new Vector2((float)Math.Cos(radians) * orbit_radius, (float)Math.Sin(radians) * orbit_radius);
            }
            else
            {
                firePlanet.Position = centerPosition;
                waterPlanet.Position = centerPosition + new Vector2((float)Math.Cos(radians) * orbit_radius, (float)Math.Sin(radians) * orbit_radius);
            }
        }
    }

    // 3. DRAWABLE REPRESENTATION FRAMEWORK
    public class DrawableOrbitRuleset : DrawableRuleset<OrbitHitObject>
    {
        public DrawableOrbitRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods) { }

        protected override Playfield CreatePlayfield() 
            => new OrbitPlayfield();

        protected override DrawableHitObject<OrbitHitObject> CreateDrawableRepresentation(OrbitHitObject h)
            => null;
    }

    // 4. DATA COMPONENT
    public class OrbitHitObject : HitObject
    {
    }

    // 5. BEATMAP CONVERSION ROUTINE
    public class OrbitBeatmapConverter : BeatmapConverter<OrbitHitObject>
    {
        public OrbitBeatmapConverter(IBeatmap beatmap, Ruleset ruleset) : base(beatmap, ruleset) { }
        public override bool CanConvert() => true;
        
        protected override IEnumerable<OrbitHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            yield return new OrbitHitObject { StartTime = original.StartTime };
        }

        protected override Beatmap<OrbitHitObject> CreateBeatmap() 
            => new Beatmap<OrbitHitObject>();
    }

    // 6. DUMMY DIFFICULTY CALCULATOR (PREVENTS FRAMEWORK BREAKAGE)
    public class OrbitDifficultyCalculator : DifficultyCalculator
    {
        public OrbitDifficultyCalculator(Ruleset ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap) { }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, System.ReadOnlySpan<Mod> mods)
            => new DifficultyAttributes();
    }
