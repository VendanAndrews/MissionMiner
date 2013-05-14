﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveCom;
using EveComFramework.Core;
using EveComFramework.Move;
using EveComFramework.Cargo;
using EveComFramework.DroneControl;
using EveComFramework.Targets;
using EveComFramework.AutoModule;
using EveComFramework.Optimizer;
using EveComFramework.Data;

namespace MissionMiner
{
    #region UI

    class MissionMinerUIData : State
    {
        public Dictionary<string, int> Agents;
        public void GetData(Action CallBack)
        {
            QueueState(GetDataState, 1, CallBack);
        }

        public bool GetDataState(object[] Params)
        {
            Agents = Agent.MyAgents.ToDictionary(a => a.Name, a => a.ID);
            ((Action)Params[0])();
            return true;
        }
    }

    #endregion

    #region Settings

    class MissionMinerSettings : Settings
    {
        public bool UnknownMissionHalt = false;
        public List<int> Levels = new List<int> { 1 };
    }

    #endregion

    class MissionMiner : State
    {
        #region Instantiation

        static MissionMiner _Instance;
        public static MissionMiner Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new MissionMiner();
                }
                return _Instance;
            }
        }

        private MissionMiner() : base()
        {
        }

        #endregion

        #region Variables

        public MissionMinerSettings Config = new MissionMinerSettings();
        Logger Console = new Logger("Miner");

        Cargo Cargo = Cargo.Instance;
        Move Move = Move.Instance;
        public AutoModule Automodule = AutoModule.Instance;
        public Optimizer Optimizer = Optimizer.Instance;

        AgentMission CurrentMission;
        MissionData CurrentMissionData;
        Agent CurrentAgent;
        List<Agent> AgentQueue;
        Dictionary<Agent, DateTime> NextDecline = new Dictionary<Agent, DateTime>();

        #endregion

        #region Actions

        public void Start()
        {
            QueueState(Offload);
            QueueState(GetNewMission, 5000);
        }

        #endregion

        #region States

        public bool CheckForMissions(object[] Params)
        {
            if (AgentMission.All.Any(a => a.Type == AgentMission.MissionType.Mining && a.State == AgentMission.MissionState.Accepted))
            {
                CurrentMission = AgentMission.All.FirstOrDefault(a => a.Type == AgentMission.MissionType.Mining && a.State == AgentMission.MissionState.Accepted);
                CurrentMissionData = MissionData.All.First(m => m.Name == CurrentMission.Name);
                Console.Log("|oMission found in journal");
                Console.Log(" |-g{0}", CurrentMission.Name);
                QueueState(Offload);
                QueueState(Traveling);
                QueueState(CheckMissionCompletion);
            }
            else
            {
                if (CurrentAgent == null)
                {
                    if (!AgentQueue.Any())
                    {
                        AgentQueue = Agent.MyAgents.Where(a => a.AgentType == Agent.AgentTypes.BasicAgent && a.AgentDivision == Agent.AgentDivisions.Mining && Config.Levels.Contains(a.Level)).ToList();
                    }
                    CurrentAgent = AgentQueue.FirstOrDefault();
                    AgentQueue.Remove(CurrentAgent);
                }
                if (Session.InSpace || Session.StationID == CurrentAgent.StationID)
                {
                    CurrentAgent.SetDestination();
                }
                QueueState(CheckLowSecAgent);
            }
            return true;
        }

        public bool CheckMissionCompletion(object[] Params)
        {
            if (Station.ItemHangar == null)
            {
                Command.OpenInventory.Execute(); 
                return false;
            } 
            if (!Station.ItemHangar.IsPrimed)
            {
                Station.ItemHangar.MakeActive();
                return false;
            }
            double MinedAmount = Station.ItemHangar.Items.Where(i => i.Type == CurrentMissionData.Asteroid).Sum(i => i.Quantity * i.Volume);
            Console.Log("|oMission Status");
            Console.Log(" |-g{0}% complete", (MinedAmount / CurrentMissionData.Volume).ToString("P0"));
            if (MinedAmount >= CurrentMissionData.Volume)
            {
                InsertState(CompleteMission, 5000);
                return true;
            }

            CurrentMission.Bookmarks.First(b => b.LocationType == "dungeon").SetDestination();
            InsertState(CheckLowSecDungeon);
            return true;
        }

        bool Offload(object[] Params)
        {
            Cargo.At(CurrentMission.Bookmarks.First(b => b.LocationType == "objective")).Unload();
            Cargo.At(CurrentMission.Bookmarks.First(b => b.LocationType == "objective"), () => MyShip.OreHold).Unload();
            return true;
        }

        bool Traveling(object[] Params)
        {
            if (!Move.Idle || !Cargo.Idle || (Session.InSpace && MyShip.ToEntity.Mode == EntityMode.Warping))
            {
                return false;
            }
            return true;
        }

        bool CheckLowSecAgent(object[] Params)
        {
            if (Route.Path.Any(a => SolarSystem.All.Any(b => b.ID == a && b.Security < .5)))
            {
                Console.Log("|yLow Security system found in Route");
                Console.Log(" |-gSkipping agent");
                CurrentAgent = null;
                QueueState(CheckForMissions);
                return true;
            }
            Move.ToggleAutopilot(true);
            QueueState(Traveling);
            QueueState(GetNewMission);
            return true;
        }

        bool CheckLowSecDungeon(object[] Params)
        {
            if (Route.Path.Any(a => SolarSystem.All.Any(b => b.ID == a && b.Security < .5)))
            {
                Console.Log("|yLow Security system found in Route");
                Console.Log(" |-gDeclining mission");

                InsertState(CheckForMissions);
                InsertState(DeclineMission);
                return true;
            }

            InsertState(CheckMissionCompletion);
            InsertState(Traveling);
            InsertState(Offload);
            InsertState(PrepWarp);
            InsertState(MineRoid);
            InsertState(Traveling);
            return true;
        }

        public bool CompleteMission(object[] Params)
        {
            AgentDialogWindow window = AgentDialogWindow.All.FirstOrDefault(w => w.AgentID == CurrentMission.AgentID);
            if (window == null)
            {
                Agent curAgent = Agent.Get(CurrentMission.AgentID);
                curAgent.StartConversation();
                return false;
            }
            window.ClickButton(Window.Button.CompleteMission);
            CurrentMission = null;
            return true;
        }

        public bool DeclineMission(object[] Params)
        {
            AgentDialogWindow window = AgentDialogWindow.All.FirstOrDefault(w => w.AgentID == CurrentAgent.ID);
            if (window == null)
            {
                CurrentAgent.StartConversation();
                return false;
            }
            if (window.HasButton(Window.Button.Decline))
            {
                window.ClickButton(Window.Button.Decline);
            }

            return true;
        }

        public bool GetNewMission(object[] Params)
        {
            AgentDialogWindow window = AgentDialogWindow.All.FirstOrDefault(w => w.AgentID == CurrentAgent.ID);
            if (window == null)
            {
                CurrentAgent.StartConversation();
                return false;
            }
            if (window.HasButton(Window.Button.RequestMission))
            {
                window.ClickButton(Window.Button.RequestMission);
                return false;
            }
            if (window.HasButton(Window.Button.Accept))
            {
                AgentMission NewMission = AgentMission.All.First(m => m.AgentID == CurrentAgent.ID);
                EVEFrame.Log(NewMission.Name);
                MissionData NewMissionData = MissionData.All.FirstOrDefault(m => m.Name == NewMission.Name);
                if (NewMissionData != null)
                {
                    window.ClickButton(Window.Button.Accept);
                }
                else
                {
                    Console.Log("|oUnknown mission detected");
                    if (Config.UnknownMissionHalt)
                    {
                        Console.Log(" |rHalted!");
                        Clear();
                        return true;
                    }
                    if (NextDecline.ContainsKey(CurrentAgent) && NextDecline[CurrentAgent] > DateTime.Now)
                    {
                        Console.Log(" |-gUnable to declining mission - on cooldown");
                        QueueState(CheckForMissions);
                        return true;
                    }

                    Console.Log(" |-gDeclining mission");
                    window.ClickButton(Window.Button.Decline);
                    NextDecline.AddOrUpdate(CurrentAgent, DateTime.Now.AddHours(4));
                }
                return false;
            }
            CurrentMission = AgentMission.All.FirstOrDefault(m => m.AgentID == CurrentAgent.ID);
            AgentMission.All.ForEach(m => EVEFrame.Log(m.Name + " " + m.AgentID));
            if (CurrentMission != null && CurrentMission.State == AgentMission.MissionState.Accepted)
            {
                CurrentMissionData = MissionData.All.First(m => m.Name == CurrentMission.Name);
                window.Close();
                QueueState(CheckMissionCompletion);
                return true;
            }
            EVEFrame.Log("Huh?");
            
            return false;
        }


        Dictionary<Module, int> CycleCounts;
        Dictionary<Module, double> CycleCompletion;
        Dictionary<Module, Entity> Targets;
        Entity Rescan;

        public bool MineRoid(object[] Params)
        {
            Func<Module, bool> Lasers = mod => mod.GroupID == Group.MiningLaser || mod.GroupID == Group.StripMiner || mod.GroupID == Group.FrequencyMiningLaser;
            if (CycleCounts == null)
            {
                CycleCounts = MyShip.Modules.Where(Lasers).ToDictionary(m => m, m => 0);
                CycleCompletion = MyShip.Modules.Where(Lasers).ToDictionary(m => m, m => 0.0);
                Targets = MyShip.Modules.Where(Lasers).ToDictionary<Module, Module, Entity>(m => m, m => null);
            }
            if (MyShip.OreHold == null)
            {
                Command.OpenInventory.Execute();
                return false;
            }
            if (!MyShip.OreHold.IsPrimed)
            {
                MyShip.OreHold.MakeActive();
                return false;
            }
            if (MyShip.OreHold.UsedCapacity >= MyShip.OreHold.MaxCapacity * 0.95)
            {
                return true;
            }
            Entity Roid;
            if (CurrentMissionData.Asteroid != "")
            {
                Roid = Entity.All.Where(ent => ent.CategoryID == Category.Asteroid && ent.Type.Contains(CurrentMissionData.Asteroid)).OrderBy(e => e.Distance).FirstOrDefault();
            }
            else
            {
                Roid = Entity.All.Where(ent => ent.CategoryID == Category.Asteroid).OrderBy(e => e.Distance).FirstOrDefault();
            }

            if(Roid != null)
            {
                DroneControl.Instance.Start();
                if(Roid.Distance > MyShip.Modules.Where(Lasers).Min(mod => mod.MaxRange) && MyShip.ToEntity.Mode == EntityMode.Stopped)
                {
                    Move.Approach(Roid);
                    return false;
                }
                if(!Roid.LockedTarget && !Roid.LockingTarget && Roid.Distance < MyShip.MaxTargetRange)
                {
                    Roid.LockTarget();
                    return false;
                }
                if (Roid.LockedTarget && Roid.Distance < MyShip.Modules.Where(Lasers).Min(mod => mod.MaxRange) && MyShip.Modules.Where(Lasers).Count(mod => !mod.IsActive) > 0)
                {
                    Module laser = MyShip.Modules.Where(Lasers).First(mod => !mod.IsActive);
                    laser.Activate(Roid);
                    Targets[laser] = Roid;
                    CycleCounts[laser] = 0;
                    CycleCompletion[laser] = 0;
                    return false;
                }

                foreach (Module laser in MyShip.Modules.Where(Lasers))
                {
                    if (Targets[laser] != Roid)
                    {
                        Targets[laser] = null;
                        CycleCounts[laser] = 0;
                        CycleCompletion[laser] = 0;
                        if (!Roid.ActiveModules.Contains(laser))
                        {
                            laser.Deactivate();
                            return false;
                        }
                    }
                    if (laser.Completion < CycleCompletion[laser])
                    {
                        CycleCounts[laser]++;
                    }
                    CycleCompletion[laser] = laser.Completion;
                }

                if (MyShip.Modules.Any(mod => mod.GroupID == Group.SurveyScanner))
                {
                    if (Rescan == Roid)
                    {
                        Module Scanner = MyShip.Modules.First(mod => mod.GroupID == Group.SurveyScanner);
                        if (!Scanner.IsActive)
                        {
                            Scanner.Activate();
                        }
                    }
                    if (!SurveyScan.Scan.ContainsKey(Roid))
                    {
                        Module Scanner = MyShip.Modules.First(mod => mod.GroupID == Group.SurveyScanner);
                        if (!Scanner.IsActive)
                        {
                            Scanner.Activate();
                        }
                    }
                    else
                    {
                        double OreMined = Roid.ActiveModules.Sum(mod => mod.MiningYield * (mod.Completion + CycleCounts[mod])).Value;
                        if (OreMined > (SurveyScan.Scan[Roid] * Roid.Volume) + 20)
                        {
                            Roid.ActiveModules.ForEach(mod => mod.Deactivate());
                            Rescan = Roid;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        public bool PrepWarp(object[] Params)
        {
            if (Busy.Instance.IsBusy)
            {
                DroneControl.Instance.Pause();
                return false;
            }
            return true;
        }


        #endregion

    }


    #region Utility classes

    static class DictionaryHelper
    {
        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }

    public static class ForEachExtension
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> method)
        {
            foreach (T item in items)
            {
                method(item);
            }
        }
    }

    #endregion

}