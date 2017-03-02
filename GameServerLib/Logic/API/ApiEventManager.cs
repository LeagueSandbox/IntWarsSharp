﻿using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.Logic.API
{
    public static class ApiEventManager
    {
        private static Game _game;
        private static Logger _logger;

        internal static void SetGame(Game game)
        {
            _game = game;
            _logger = Program.ResolveDependency<Logger>();
        }

        public static void removeAllListenersForOwner(Object owner)
        {
            OnChampionDamaged.RemoveListener(owner);
            OnUpdate.RemoveListener(owner);
        }
        
        public static EventOnUpdate OnUpdate = new EventOnUpdate();
        public static EventOnChampionDamaged OnChampionDamaged = new EventOnChampionDamaged();
        public static EventOnUnitDamaged OnUnitDamaged = new EventOnUnitDamaged();
    }


    public class EventOnUpdate
    {
        private List<Tuple<Object, Action<float>>> listeners = new List<Tuple<object, Action<float>>>();
        public void AddListener(Object owner, Action<float> callback)
        {
            var listenerTuple = new Tuple<object, Action<float>>(owner, callback);
            listeners.Add(listenerTuple);
        }

        public void RemoveListener(Object owner)
        {
            listeners.RemoveAll((listener) => listener.Item1 == owner);
        }
        public void Publish(float diff)
        {
            listeners.ForEach((listener) => {
                listener.Item2(diff);
            });
        }
    }

    public class EventOnUnitDamaged
    {
        private List<Tuple<Object, Unit, Action>> listeners = new List<Tuple<object, Unit, Action>>();
        public void AddListener(Object owner, Unit unit, Action callback)
        {
            var listenerTuple = new Tuple<object, Unit, Action>(owner, unit, callback);
            listeners.Add(listenerTuple);
        }

        public void RemoveListener(Object owner, Unit unit)
        {
            listeners.RemoveAll((listener) => listener.Item1 == owner && listener.Item2 == unit);
        }
        public void RemoveListener(Object owner)
        {
            listeners.RemoveAll((listener) => listener.Item1 == owner);
        }
        public void Publish(Unit unit)
        {
            listeners.ForEach((listener) => {
                listener.Item3();
            });
            if (unit is Champion)
            {
                ApiEventManager.OnChampionDamaged.Publish((Champion)unit);
            }
        }
    }

    public class EventOnChampionDamaged
    {
        private List<Tuple<Object, Champion, Action>> listeners = new List<Tuple<object, Champion, Action>>();
        public void AddListener(Object owner, Champion champion, Action callback)
        {
            var listenerTuple = new Tuple<object, Champion, Action>(owner, champion, callback);
            listeners.Add(listenerTuple);
        }

        public void RemoveListener(Object owner, Champion champion)
        {
            listeners.RemoveAll((listener) => listener.Item1 == owner && listener.Item2 == champion);
        }
        public void RemoveListener(Object owner)
        {
            listeners.RemoveAll((listener) => listener.Item1 == owner);
        }
        public void Publish(Champion champion)
        {
            listeners.ForEach((listener) => {
                if (listener.Item2 == champion)
                {
                    listener.Item3();
                }
            });
        }
    }
}