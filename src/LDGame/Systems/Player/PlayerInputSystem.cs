using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Geometry;
using Murder.Components;
using Murder;
using Bang.Components;
using Murder.Helpers;
using LDGame.Components;
using LDGame.Messages;
using LDGame.Core;
using LDGame.Services;
using Murder.Services;
using LDGame.Core.Sounds;
using Murder.Utilities;

namespace LDGame.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(PlayerComponent))]
    public class PlayerInputSystem : IUpdateSystem, IFixedUpdateSystem
    {
        private Vector2 _cachedInputAxis = Vector2.Zero;
        private int _cachedInputSkill = -1;
        
        private bool _previousCachedAttack = false;
        private bool _cachedAttack = false;

        private bool _interacted = false;
        
        /// <summary>
        /// Whether the player locked the movement and only wants to change the facing.
        /// </summary>
        private bool _lockMovement = false;
        private Vector2 _forcedInput = Vector2.Zero;
        private Vector2 _previousInput = Vector2.Zero;
        private float _nextSound = 0;

        public void FixedUpdate(Context context)
        {
            var save = SaveServices.GetOrCreateSave();
            
            foreach (Entity entity in context.Entities)
            {
                PlayerComponent player = entity.GetComponent<PlayerComponent>();
                if (player.CurrentState != PlayerStates.Normal)
                {
                    // Skip movement if the player is casting a spell
                    continue;
                }

                bool moved = _cachedInputAxis.Manhattan() != 0;
                Vector2 impulse = Vector2.Zero;
                
                if (_interacted)
                {
                    entity.SendMessage<InteractorMessage>();
                }

                Direction direction;
                if (moved)
                {
                    _forcedInput = Vector2.Zero;
                    direction = DirectionHelper.FromVector(_cachedInputAxis);
                    impulse = _lockMovement ? Vector2.Zero : _cachedInputAxis;
                    entity.SetAgentImpulse(impulse, direction);
                }
                else
                {
                    _forcedInput = save.SwayDirection;
                    _forcedInput = _forcedInput.ClampMagnitude(1);

                    direction = DirectionHelper.FromVector(_forcedInput);
                    impulse = _lockMovement ? Vector2.Zero : _forcedInput;
                    if (impulse.HasValue)
                        entity.SetAgentImpulse(impulse, direction);
                }

                // Play animations
                if (entity.HasBoost())
                {
                    entity.SetAgentImpulse(impulse + new Vector2(0,0f), direction);
                    entity.SetPlayerSpeed(6f);

                    entity.PlayAsepriteAnimation("boost");
                }
                else
                {
                    switch (impulse.X)
                    {
                        case < 0:
                            entity.PlayAsepriteAnimation("left");
                            break;
                        case > 0:
                            entity.PlayAsepriteAnimation("right");
                            break;
                        default:
                            entity.PlayAsepriteAnimation("idle");
                            break;
                    }
                }

                if (_cachedInputSkill > 0)
                {
                    entity.SendMessage(new AgentInputMessage(_cachedInputSkill));
                }
                _cachedInputSkill = -1;
                
                if (!_previousCachedAttack && _cachedAttack)
                {
                    _previousCachedAttack = true;
                    entity.SendMessage(new AgentInputMessage(InputButtons.Attack));
                }
                else if (_previousCachedAttack && !_cachedAttack)
                {
                    _previousCachedAttack = false;
                    entity.SendMessage(new AgentReleaseInputMessage(InputButtons.Attack));
                }
                _interacted = false;

                // [HACK]
                // Increase player speed
                entity.SetPlayerSpeed(entity.GetPlayerSpeed().Approach(3f, 1f * Game.FixedDeltaTime));   
            }

        }
        public void Update(Context context)
        {
            _cachedInputAxis = Game.Input.GetAxis(InputAxis.Movement).Value;
            
            _lockMovement = Game.Input.Down(InputButtons.LockMovement);

            _cachedAttack = Game.Input.Down(InputButtons.Attack);
            
            if (Game.Input.Pressed(InputButtons.Interact))
            {
                _interacted = true;
            }
            else if (Game.Input.Pressed(InputButtons.Pause) && 
                !context.World.IsPaused && context.World.GetEntitiesWith(typeof(FadeTransitionComponent)).Count() == 0)
            {
                LibraryServices.GetPausePrefab().Create(context.World);

                Game.Input.Consume(InputButtons.Pause);
                Game.Input.Consume(InputButtons.Cancel);

                LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().UiBack, isLoop: false);
            }

            if (_nextSound < Game.Now)
            {
                if (_cachedInputAxis.X != 0 && _previousInput.X != _cachedInputAxis.X)
                {

                    if (Game.Random.TryWithChanceOf(0.1f))
                    {
                        LDGameSoundPlayer.Instance.PlayEvent(LibraryServices.GetRoadLibrary().CarTireSqueal, isLoop: false);
                        _nextSound = Game.Now + 1.1f;
                    }
                }
            }

            _previousInput = _cachedInputAxis;

        }
    }
}
