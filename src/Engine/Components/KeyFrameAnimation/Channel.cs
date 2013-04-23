using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Fusee.KeyFrameAnimation
{
    public class Channel
    {
        private List<Keyframe> _timeline;
        private IComparer<Keyframe> _sort;

        //TODO The First Keyframe should have the start value of the Objekt. Shoukd be done by "Channelmanager / Animation".
        public Channel()
        {
            _timeline = new List<Keyframe>();
            _sort = new KeyFrameSort();
            _timeline.Add(new Keyframe());
        }


        //Add Keyframes 

        public void AddKeyframe(Keyframe keyframe)
        {
            bool create = true;

            for (int next = 0; next < _timeline.Count; next++)
            {
                if (keyframe.Time == _timeline[next].Time)
                {
                    create = false;
                }
            }

            if (create)
            {
                _timeline.Add(keyframe);
                SortTimeline();
            }
            //else //TODO necessary?
            //{
            //    keyframe = null;
            //}
        }

        //Maybe we schould place here the current values of the animated Object as a startvalue, therefore we need them first.
        public void AddKeyframe()
        {

            if (_timeline != null)
            {
                bool create = true;

                if (_timeline[0].Time == 0)
                {
                    create = false;
                }

                if (create)
                {
                    _timeline.Add(new Keyframe());
                    SortTimeline();
                }
            }
        }

        public void AddKeyframe(float time)
        {
            bool create = true;

            for (int next = 0; next < _timeline.Count; next++)
            {
                if (time == _timeline[next].Time)
                {
                    create = false;
                }
            }

            if (create)
            {
                _timeline.Add(new Keyframe(time));
                SortTimeline();
            }

        }

        public void AddKeyframe(float time, float value)
        {
            bool create = true;

            for (int next = 0; next < _timeline.Count; next++)
            {
                if (time == _timeline[next].Time)
                {
                    create = false;
                    //Console.WriteLine("worked.................");
                }
            }

            if (create)
            {
                _timeline.Add(new Keyframe(time, value));
                SortTimeline();
            }
        }

        //Remove KEyframes
        public void RemoveKeyframe(float time)
        {

            foreach (Keyframe keyframe in _timeline)
            {
                if (keyframe.Time == time)
                {
                    _timeline.Remove(keyframe);
                }
            }
        }

        //Returns the value of a keyframe at a specific time
        public float GetValueAt(float time)
        {
            float keyValue = 0;

            for (int next = 1; next < _timeline.Count; next++)
            {
               
                if (_timeline[next].Time > time && _timeline[next - 1].Time < time)
                {
                    /*Console.WriteLine("_timeline[next - 1].Time = " + _timeline[next - 1].Time);
                    Console.WriteLine("_timeline[next - 1].Value = " + _timeline[next - 1].Value);
                    Console.WriteLine("_timeline[next].Time = " + _timeline[next].Time);
                    Console.WriteLine("_timeline[next].Value = " + _timeline[next].Value);
                    /* Console.WriteLine("_timeline[0].Time = " + _timeline[0].Time);
                    Console.WriteLine("_timeline[0].Time = " + _timeline[0].Value);
                    Console.WriteLine("_timeline[1].Time = " + _timeline[1].Time);
                    Console.WriteLine("_timeline[1].Time = " + _timeline[1].Value);
                    Console.WriteLine("_timeline[2].Time = " + _timeline[2].Time);
                    Console.WriteLine("_timeline[2].Time = " + _timeline[2].Value);
                    */

                    keyValue = _timeline[next - 1].Value +
                               (((_timeline[next].Value - _timeline[next - 1].Value) /
                                 (_timeline[next].Time - _timeline[next - 1].Time)) * (time - _timeline[next - 1].Time));
                    break;
                }
            }
            return keyValue;
        }
 
        void SortTimeline()
        {
            if (_sort != null) _timeline.Sort(_sort);
        }
    }
}
