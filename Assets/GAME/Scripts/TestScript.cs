using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class TestScript : MonoBehaviour
{
    public class TestModel
    {
        public string Id { get; set; }
        public int HP { get; set; }
    }

    private void Start()
    {
        var characterModels = new List<CharacterModel>
        {
            new CharacterModel
            {
                Id = 1.ToString(),
                HP = new ReactiveProperty<int>(1),
                IsPlayer = true,
                Damage = 999
            },
            new CharacterModel
            {
                Id = 2.ToString(),
                HP = new ReactiveProperty<int>(2),
                IsPlayer = false,
                Damage = 888
            }
        };

        var testModels = new List<TestModel>
        {
            new TestModel
            {
                Id = 1.ToString(),
                HP = 1
            },
            new TestModel
            {
                Id = 2.ToString(),
                HP = 1
            }
        };


        characterModels.ToObservable()
            .Subscribe(model => { Debug.Log($"model with id: {model.Id}"); });

        testModels.ToObservable()
            .Subscribe(model => { model.Id = 3.ToString(); });
        testModels.ToObservable()
            .Subscribe(model => { Debug.Log($"model with id: {model.Id}"); });
    }
}