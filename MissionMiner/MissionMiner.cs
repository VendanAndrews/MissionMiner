using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveCom;
using EveComFramework.Core;
using EveComFramework.Move;
using EveComFramework.Cargo;
using EveComFramework.DroneControl;
using EveComFramework.Targets;

namespace MissionMiner
{
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

    class MissionMinerSettings : Settings
    {
        public string Agent;
    }

    class MissionMiner : State
    {
        AgentMission CurrentMission;
        MissionData CurrentMissionData;
        public MissionMinerSettings Settings = new MissionMinerSettings();

        public void Start()
        {
            QueueState(Offload);
            QueueState(GetNewMission, 5000);
        }

        public bool Offload(object[] Params)
        {
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
            if (MyShip.OreHold.Items.Count > 0)
            {
                EVEFrame.Log("Priming OreHold");
                MyShip.OreHold.Items.MoveTo(Station.ItemHangar);
                return false;
            }

            if (MyShip.CargoBay == null)
            {
                Command.OpenInventory.Execute();
                return false;
            }
            if (!MyShip.CargoBay.IsPrimed)
            {
                EVEFrame.Log("Priming CargoBay");
                MyShip.CargoBay.MakeActive();
                return false;
            }
            if (MyShip.CargoBay.Items.Count > 0)
            {
                MyShip.CargoBay.Items.MoveTo(Station.ItemHangar);
                return false;
            }

            return true;
        }

        public bool CompleteMission(object[] Params)
        {
            if (AgentMission.NeedUpdate)
            {
                AgentMission.Update();
                return false;
            }
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

        public bool GetNewMission(object[] Params)
        {
            if (AgentMission.NeedUpdate)
            {
                AgentMission.Update();
                return false;
            }
            Agent newAgent = Agent.MyAgents.First(a => a.Name == Settings.Agent);
            AgentDialogWindow window = AgentDialogWindow.All.FirstOrDefault(w => w.AgentID == newAgent.ID);
            if (window == null)
            {
                newAgent.StartConversation();
                return false;
            }
            if (window.HasButton(Window.Button.RequestMission))
            {
                window.ClickButton(Window.Button.RequestMission);
                return false;
            }
            if (window.HasButton(Window.Button.Accept))
            {
                AgentMission NewMission = AgentMission.All.First(m => m.AgentID == newAgent.ID);
                EVEFrame.Log(NewMission.Name);
                MissionData NewMissionData = MissionData.All.FirstOrDefault(m => m.Name == NewMission.Name);
                if (NewMissionData != null)
                {
                    window.ClickButton(Window.Button.Accept);
                }
                else
                {
                    window.ClickButton(Window.Button.Decline);
                }
                return false;
            }
            CurrentMission = AgentMission.All.FirstOrDefault(m => m.AgentID == newAgent.ID);
            AgentMission.All.ForEach(m => EVEFrame.Log(m.Name + " " + m.AgentID));
            if (CurrentMission != null && CurrentMission.State == AgentMission.MissionState.Accepted)
            {
                CurrentMissionData = MissionData.All.First(m => m.Name == CurrentMission.Name);
                window.Close();
                QueueState(RunMiningMission);
                QueueState(GetNewMission, 5000);
                return true;
            }
            EVEFrame.Log("Huh?");
            
            return false;
        }

        public bool Undock(object[] Params)
        {
            if (Session.InStation)
            {
                Command.CmdExitStation.Execute();
                InsertState(Undock);
                WaitFor(30, () => Session.InSpace);
            }
            return true;
        }

        public bool RunMiningMission(object[] Params)
        {
            if (!Station.ItemHangar.IsPrimed)
            {
                EVEFrame.Log("Priming ItemHangar");
                Station.ItemHangar.MakeActive();
                return false;
            }
            double MinedAmount = Station.ItemHangar.Items.Where(i => i.Type == CurrentMissionData.Asteroid).Sum(i => i.Quantity * i.Volume);
            EVEFrame.Log(MinedAmount.ToString() + " of " + CurrentMissionData.Volume.ToString());
            if (MinedAmount >= CurrentMissionData.Volume)
            {
                InsertState(CompleteMission, 5000);
                return true;
            }
            InsertState(RunMiningMission);
            InsertState(Offload);
            InsertState(ReturnToAgent);
            InsertState(PrepWarp);
            InsertState(MineRoid);
            InsertState(WarpToObjective);
            InsertState(Undock);

            return true;
        }

        public bool WarpToObjective(object[] Params)
        {
            if (AgentMission.NeedUpdate)
            {
                AgentMission.Update();
                return false;
            }
            Move.Instance.Bookmark(CurrentMission.Bookmarks.First(b => b.LocationType == "dungeon"));
            WaitFor(10, () => false, () => !Move.Instance.Idle || MyShip.ToEntity.Mode == EntityMode.Warping);
            return true;
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
                    Roid.Approach();
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
                return false;
            }
            DroneControl.Instance.Clear();
            return true;
        }

        public bool ReturnToAgent(object[] Params)
        {
            if (AgentMission.NeedUpdate)
            {
                AgentMission.Update();
                return false;
            }
            Move.Instance.Bookmark(CurrentMission.Bookmarks.First(b => b.LocationType == "objective"));
            WaitFor(60, () => Session.InStation, () => !Move.Instance.Idle || MyShip.ToEntity.Mode == EntityMode.Warping);
            return true;
        }
    }
}
