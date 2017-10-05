using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

    public partial class Gameboard {

        private class TimeTrack
        {

            private int _time_start;
            private int _time_current;
            private int _sudden_death;

            private Dictionary<int, Dictionary<Nationality, Stack>> reinforcements;

            public TimeTrack(int start, int sudden_death) {
                Start = start;
                Time = Start;
                SuddenDeath = sudden_death;

                reinforcements = new Dictionary<int, Dictionary<Nationality, Stack>>();
            }

            private bool inBounds(int time) {
                //TODO: check upper bounds
                return time >= 0;
            }

            public int Start {
                set {
                    if (inBounds(value))
                        _time_start = value;
                    else
                        throw new ArgumentOutOfRangeException(String.Format("Time marker cannot start out of bounds ({0})",value));
                }
                get { return _time_start; }
            }

            public int Time {
                set {
                    if (inBounds(value))
                        _time_current = value;
                    else
                        throw new ArgumentOutOfRangeException(String.Format("Time marker cannot be placed out of bounds ({0})",value));
                }
                get { return _time_current; }
            }

            public int SuddenDeath {
                set {
                    if (inBounds(value))
                        _sudden_death = value;
                    else
                        throw new ArgumentOutOfRangeException(String.Format("Sudden Death cannot be out of bounds ({0})", value));
                }
                get { return _sudden_death; }
            }

            public int Advance() {
                return ++Time;
            }

            public void AddReinforcements(int time_space, Unit u) {
                if (time_space >= 0) {
                    if (!reinforcements.ContainsKey(time_space))
                        reinforcements.Add(time_space, new Dictionary<Nationality, Stack>());

                    if (!reinforcements[time_space].ContainsKey(u.Nation))
                        reinforcements[time_space].Add(u.Nation, new Stack());

                    reinforcements[time_space][u.Nation].Add(u);

                }
                else
                    throw new ArgumentOutOfRangeException("Reinforcements cannot be placed below 0");
            }


            public bool InSuddenDeath() {
                return Time >= SuddenDeath;
            }

            public Dictionary<Nationality, Stack> TakeReinforcements(int time_space) {
                Dictionary<Nationality, Stack> r;
                reinforcements.TryGetValue(time_space, out r);
                if (r != null)
                    reinforcements.Remove(time_space);
                return r;
            }

        }

    }
}
