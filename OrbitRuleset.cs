using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using System.Linq;
using osuTK;
using osuTK.Graphics;

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

        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.CircleNotch
        };

        public override IConvertibleRebindings<OrbitAction> GetReceiver() => new RebindableKeys<OrbitAction>(new()
        {
            { OrbitAction.Button1, new[] { InputKey.Z, InputKey.MouseLeft } },
            { OrbitAction.Button2, new[] { InputKey.X, InputKey.MouseRight } }
        });
    }

    public class OrbitPlayfield : Playfield
    {
        private readonly Circle firePlanet;
        private readonly Circle waterPlanet;
        
        private const float orbit_radius = 80f;
        private Vector2 centerPosition = new Vector2(512, 384);
        private bool isWaterPivot = true;

        public System.Collections.Generic.IEnumerable<Drawable> AliveInternalChildren => AliveChildren;

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

    public class DrawableOrbitRuleset : DrawableRuleset<OrbitHitObject>
    {
        private OrbitPlayfield orbitPlayfield;

        public DrawableOrbitRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods) { }

        protected override Playfield CreatePlayfield() => orbitPlayfield = new OrbitPlayfield();
        protected override PlayfieldRenderer CreatePlayfieldRenderer() => new PlayfieldRenderer();
        protected override PassThroughInputManager CreateInputManager() => new OrbitInputManager(Ruleset.RulesetInfo);
        protected override DrawableHitObject<OrbitHitObject> CreateDrawableRepresentation(OrbitHitObject h) => new DrawableOrbitHitObject(h);

        public bool OnPressed(KeyBindingPressEvent<OrbitAction> e)
        {
            orbitPlayfield?.SwitchPlanetPivot();
            var nextNote = orbitPlayfield?.AliveInternalChildren.OfType<DrawableOrbitHitObject>().FirstOrDefault(n => !n.Result.HasResult);
            if (nextNote != null) nextNote.TriggerResult();
            return true;
        }
    }

    public class OrbitHitObject : HitObject, IHasPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Position => new Vector2(X, Y);
    }

    public class DrawableOrbitHitObject : DrawableHitObject<OrbitHitObject>
    {
        public DrawableOrbitHitObject(OrbitHitObject hitObject) : base(hitObject)
        {
            Size = new Vector2(40);
            Origin = Anchor.Centre;
            Position = hitObject.Position;
            Alpha = 0;
            Child = new Circle { RelativeSizeAxes = Axes.Both, Colour = Color4.White, Anchor = Anchor.Centre, Origin = Anchor.Centre };
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (timeOffset > 150) ApplyResult(HitResult.Miss);
                return;
            }
            double absOffset = Math.Abs(timeOffset);
            if (absOffset <= 40) ApplyResult(HitResult.Great);
            else if (absOffset <= 90) ApplyResult(HitResult.Ok);
            else if (absOffset <= 150) ApplyResult(HitResult.Meh);
            else ApplyResult(HitResult.Miss);
        }

        protected override void UpdateInitialTransforms() { base.UpdateInitialTransforms(); this.FadeIn(500); }
    }

    public class OrbitBeatmapConverter : BeatmapConverter<OrbitHitObject>
    {
        public OrbitBeatmapConverter(IBeatmap beatmap, Ruleset ruleset) : base(beatmap, ruleset) { }
        public override bool CanConvert() => true;
        protected override IEnumerable<OrbitHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            float posX = 512, posY = 384;
            if (original is IHasPosition positionable) { posX = positionable.X; posY = positionable.Y; }
            yield return new OrbitHitObject { StartTime = original.StartTime, X = posX, Y = posY };
        }
        protected override Beatmap<OrbitHitObject> CreateBeatmap() => new Beatmap<OrbitHitObject>();
    }

    public class OrbitDifficultyCalculator : DifficultyCalculator
    {
        public OrbitDifficultyCalculator(Ruleset ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap) { }
        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate) => new DifficultyAttributes();
        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate) => Array.Empty<Skill>();
    }
}

