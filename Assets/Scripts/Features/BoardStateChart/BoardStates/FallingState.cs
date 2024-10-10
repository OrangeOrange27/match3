using System;
using Features.Data.Components;
using Features.Models;
using Features.Signals;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Features.BoardStateChart.BoardStates
{
    public class FallingState : BoardBaseState
    {
        [Inject] 
        private EntityManager entityManager;
        
        [Inject] 
        private BoardModel boardModel;
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            signalBus.Fire(new Match3Signals.StartFallSignal());
            
            for (var x = 0; x < boardModel.BoardWidth; x++)
            {
                for (var y = 0; y < boardModel.BoardHeight; y++)
                {
                    if (!boardModel.HasEntityAt(new int2(x, y))) 
                        continue;
                        
                    var entity = boardModel.GetEntityAt(x, y);
                    try
                    {
                        entityManager.AddComponent<IsFallingComponent>(entity);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
    }
}