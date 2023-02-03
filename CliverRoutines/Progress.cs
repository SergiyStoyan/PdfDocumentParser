//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Cliver
{
    /// <summary>
    /// Usage example
    /// </summary>
    class ProgressExample : Progress
    {
        readonly public AsymptoticStage _LoadingPOs = new AsymptoticStage { Step = 100, Weight = 1, Delta = 1000 };
        readonly public Stage _LoadingInvoices = new Stage { Step = 10, Weight = 2, Maximum = 12345 };
        readonly public Stage _Recording = new Stage { Step = 5, Weight = 4, Maximum = 5000 };

        void exampleCode()
        {
            ProgressExample progress = new ProgressExample() { Maximum = 1000 };
            progress.OnProgress += delegate (Stage stage)
            {
                //MainForm.This.SetProgress(progress.GetProgress(), ((CustomStage)stage).ItemName, stage.Maximum, stage.Value);
            };
            //...
            progress._LoadingPOs.Maximum = 100;
            for (int i = 1; i <= 100; i++)
                progress._LoadingPOs.Value = i;
        }
    }

    /// <summary>
    /// Used to display progress bar with multiple tasks.
    /// </summary>
    public class Progress
    {
        public class Stage
        {
            public string Name { get; internal set; }

            /// <summary>
            /// Can be any. Important is the ratio between all the weights.
            /// </summary>
            public float Weight
            {
                get
                {
                    return weight;
                }
                set
                {
                    if (value <= 0)
                        throw new Exception("Weight cannot be <= 0");
                    weight = value;
                }
            }
            float weight = 1;

            virtual public int Maximum
            {
                get
                {
                    return maximum;
                }
                set
                {
                    if (value < 0)
                        throw new Exception("Maximum cannot be set < 0");
                    maximum = value;
                }
            }
            int maximum = -1;

            virtual public int Value
            {
                get
                {
                    return value;
                }
                set
                {
                    lock (this)
                    {
                        //if (value == Value)
                        //    return;
                        if (value < 0)
                            throw new Exception("Value cannot be set < 0");
                        if (value > Maximum)
                            throw new Exception("Value cannot be > Maximum: " + value + " > " + Maximum);
                        this.value = value;
                        if ((value % Step == 0 /*|| value == 0*/ || value == Maximum)
                            && progress.OnProgress != null
                            )
                            progress.OnProgress(this);
                    }
                }
            }
            int value = -1;

            public uint Step = 1;

            internal Progress progress;

            public Stage(string name = null)
            {
                Name = name;
            }

            public void Complete()
            {
                if (Maximum < 0)
                    Maximum = 0;
                Value = Maximum;
            }

            /// <summary>
            /// Makes this Stage not touched yet.
            /// </summary>
            virtual public void Reset()
            {
                maximum = -1;//makes this Stage not initialized
                value = -1;
            }

            /// <summary>
            /// [0:1]
            /// </summary>
            /// <returns>[0:1]</returns>
            virtual public float GetValue1()
            {
                lock (this)
                {
                    if (Maximum < 0 || Value < 0)
                        return 0;
                    if (Maximum == 0 && Value == 0)
                        return 1;
                    return (float)Value / Maximum;
                }
            }

            public enum States
            {
                Unset,
                Set,
                Started,
                Completed,
            }

            virtual public States State
            {
                get
                {
                    if (Maximum < 0)
                        return States.Unset;
                    if (Value < 0)
                        return States.Set;
                    if (Value != Maximum)
                        return States.Started;
                    return States.Completed;
                }
            }
        }

        /// <summary>
        /// Used when Maximum cannot be determined at the beginning.
        /// </summary>
        public class AsymptoticStage : Stage
        {
            override public int Maximum
            {
                set
                {
                    throw new Exception("Maximum must not be set in this type.");
                }
            }

            override public int Value
            {
                set
                {
                    lock (this)
                    {
                        Maximum = (int)(value + Delta);
                        base.Value = value;
                    }
                }
            }

            public float Delta
            {
                get
                {
                    return delta;
                }
                set
                {
                    lock (this)
                    {
                        if (value < 0)
                            throw new Exception("Delta cannot be set < 0");
                        delta = value;
                    }
                }
            }
            protected float delta = -1;

            public AsymptoticStage(string name = null) : base(name)
            {
            }

            /// <summary>
            /// Makes this Stage not touched yet.
            /// </summary>
            override public void Reset()
            {
                delta = -1;//makes this Stage not initialized
                base.Reset();
            }

            override public States State
            {
                get
                {
                    if (Delta < 0)
                        return States.Unset;
                    if (Value < 0)
                        return States.Set;
                    if (Value != Maximum)
                        return States.Started;
                    return States.Completed;
                }
            }
        }

        //public Progress(params Stage[] stages)
        //{
        //    this.stages = stages.ToList();
        //    this.stages.ForEach(a => a.progress = this);
        //}   

        //public Stage this[string stageName]
        //{
        //    get
        //    {
        //        return stages.FirstOrDefault(a => a.Name == stageName);
        //    }
        //}

        /// <summary>
        /// Auto-detects Stages in the deriving custom class
        /// </summary>
        public Progress()
        {
            Stages = GetType()
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(a => typeof(Stage).IsAssignableFrom(a.FieldType))
                .Select(a =>
                {
                    Stage s = (Stage)a.GetValue(this);
                    if (s.Name == null)
                        s.Name = a.Name;
                    s.progress = this;
                    return s;
                })
                .ToList().AsReadOnly();
        }

        readonly public System.Collections.ObjectModel.ReadOnlyCollection<Stage> Stages;

        public event Action<Stage> OnProgress;

        public void Reset()
        {
            foreach (Stage s in Stages)
                s.Reset();
        }

        /// <summary>
        /// [0:1]
        /// </summary>
        /// <returns>[0:1]</returns>
        public float GetValue1()
        {
            lock (this)
            {
                return Stages.Sum(a => a.Weight * a.GetValue1()) / Stages.Sum(a => a.Weight);
            }
        }

        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                if (value < 0)
                    throw new Exception("Maximum cannot be < 0");
                maximum = value;
            }
        }
        int maximum = 0;

        /// <summary>
        /// [0:Maximum]
        /// </summary>
        /// <returns>[0:Maximum]</returns>
        public int GetValue()
        {
            return (int)(Maximum * GetValue1());
        }
    }
}