using Core.StateMachine;
using Features.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Features.Signals
{
    public class Match3Signals
    {
        public class CreateBoardSignal
        {
        }

        public class OnBoardCreatedSignal
        {
        }

        public class FindMatchesSignal
        {
        }

        public class StartFallSignal
        {
        }

        public class OnPlayerTurnStart
        {
        }

        public class StateChartSignal
        {
            public readonly IStateMachineEvent _stateMachineEvent;

            public StateChartSignal(IStateMachineEvent stateMachineEvent)
            {
                this._stateMachineEvent = stateMachineEvent;
            }
        }

        public class StartSwapSignal
        {
            public readonly int2 FirstSwapPosition;
            public readonly int2 SecondSwapPosition;
            public readonly bool isRevertSwap;

            public StartSwapSignal(int2 firstSwapPosition, int2 secondSwapPosition, bool isRevertSwap = false)
            {
                FirstSwapPosition = firstSwapPosition;
                SecondSwapPosition = secondSwapPosition;
                this.isRevertSwap = isRevertSwap;
            }
        }

        public class PlayerTimeChangedSignal
        {
        }
        
        public class PlayerScoreChangedSignal
        {
        }

        public class OutOfTimeSignal
        {
        }

        public class GemCreatedSignal
        {
            public readonly Entity Entity;
            public readonly GemColor GemColor;
            public readonly float3 Position;

            public GemCreatedSignal(Entity gemEntity, GemColor color, float3 position)
            {
                Entity = gemEntity;
                GemColor = color;
                Position = position;
            }
        }

        public class GemDestroyedSignal
        {
            public readonly Entity Entity;

            public GemDestroyedSignal(Entity gemEntity)
            {
                Entity = gemEntity;
            }
        }
    }
}