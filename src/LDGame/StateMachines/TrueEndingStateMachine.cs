using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using LDGame.Assets;
using LDGame.Core;
using LDGame.Core.Sounds;
using LDGame.Services;
using Murder;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;

namespace LDGame.StateMachines
{
    internal class TrueEndingStateMachine : StateMachine
    {
        private string _text =
@"Daggers McBlack may have succeeded in making Neo City Express 
the city's premiere delivery service, but at what cost did it come?

His example of enhancing performance through the use of 
psychoactive substance, texting while driving, and being extremely
horny, all contributed to a culture within his company that allows 
and accepts reckless behavior. 

His story is but one of many: to this day, hundreds of couriers all 
over Neo City are taking unnecessary risks while making deliveries. 

And while the result is that deliveries are arriving extremely quickly, 
the fact is that using drugs to make deliveries is cheating. 

Neo City bylaw is extremely clear that no delivery driver is to, at any 
point while on the job, be under the influence of designer drugs such as:

NeuroBoost
k-HyperX
Nitro Bliss
Soma 29-31
Mylk
Syno Nerv 2
Omni-Triterozine
Soma 31-37
Cryo-Steel Blues Powder
EX-Synapse Juice Ultra
TechnoJazz
Slam Spit
Neocortical-Oxide Version B
Rabbit Hat Love Fibers
Soma 37-61
TCJ-SMR
The Stuff
HyperJet 9
Pixie Tears
RetroStim
Angel Babywarp
Slo-Go Sticks
Rez-F-Q
and Mr Mindbrain's Magical Colorcubes

If you or someone you know has a story similar to that of Daggers McBlack, 
if what you've seen here today feels all too familiar, 
it's not too late to reach out for help. 
 
Call toll free:  
288-716-5191-
323-8742-6109-
8-712-719-713

Extension 714-712-719-7-7-7

Ask for Linda.

Remember, it doesn't have to be this way.
Daggers McBlack can be the end, and not the beginning.
Let's all work together to create the Neo City we want to see.
  
This video game paid for by the Federal Bureau of Neo City Council.";

        public TrueEndingStateMachine()
        {
            State(Main);
        }

        private Vector2 _cameraPosition = Vector2.Zero;

        private IEnumerator<Wait> Main()
        {
            Entity.SetCustomDraw(Draw);

            Entity.SetFadeScreen(Murder.Components.FadeType.Out, Game.Now, 1, Palette.Colors[24], destroyAfterFinished: false, customTexture: string.Empty /* unused */);
            yield return Wait.ForSeconds(1);

            World.RunCoroutine(Narrator());

            while (true)
            {
                _cameraPosition.Y += 0.35f;

                Camera2D camera = ((MonoWorld)World).Camera;
                camera.Position = _cameraPosition;

                yield return Wait.ForSeconds(0.05f);
            }
        }

        private static IEnumerator<Wait> Narrator()
        {
            LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().TrueEndingNarrator, isLoop: true);

            yield return Wait.ForSeconds(142);
            LevelServices.SwitchScene(LibraryServices.GetRoadLibrary().MainMenu);
        }

        private void Draw(RenderContext render)
        {
            LibraryAsset library = LibraryServices.GetRoadLibrary();

            int width = render.Camera.Width;
            if (DialogueServices.GetSpriteAssetForSituation(library.CityImage) is (SpriteAsset asset, string animation))
            {
                RenderServices.DrawSprite(
                    render.GameUiBatch,
                    new Vector2(54, 0),
                    0,
                    animation,
                    asset,
                    0,
                    Color.White,
                    .3f,
                    false);
            }

            Game.Data.LargeFont.Draw(render.GameUiBatch, _text, new Vector2(width / 2f, 355), new Vector2(.5f, 0), sort: .5f, Palette.Colors[20], null, null, width - 350, doLineWrapping: false);
        }
    }
}
