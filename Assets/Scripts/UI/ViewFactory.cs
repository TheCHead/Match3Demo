using System;
using System.Collections.Generic;
using Scripts.UI.Views;
using UnityEngine;

namespace Scripts.UI
{
    public class ViewFactory
    {
        private readonly Dictionary<Type, string> _viewPaths = new()
        {
            { typeof(ScoreView), "UI/Views/ScoreView" },
        };
        private readonly Dictionary<string, GameObject> _cachedPrefabs = new();
        private Transform _container;

        public ViewFactory(Transform container)
        {
            _container = container;
        }

        public TView GetView<TView>() where TView : IView
        {
            var prefabPath = _viewPaths.GetValueOrDefault(typeof(TView));

            if (prefabPath == null)
            {
                throw new Exception($"No path registered for view type {typeof(TView)}");
            }

            var prefab = LoadPrefab(prefabPath);
            var viewInstance = GameObject.Instantiate(prefab, _container).GetComponent<TView>();

            return viewInstance;
        }

        private GameObject LoadPrefab(string path)
        {
            if (!_cachedPrefabs.TryGetValue(path, out var prefab))
            {
                prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                    throw new Exception($"Failed to load UI prefab at path: {path}");

                _cachedPrefabs[path] = prefab;
            }
            return prefab;
        }
    }
}
