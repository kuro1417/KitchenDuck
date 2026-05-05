using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgess 
{
    public event EventHandler<OnProgessChangedEventArgs> OnProgessChanged;

    public class OnProgessChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}
