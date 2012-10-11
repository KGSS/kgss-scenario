using System;
using System.Collections.Generic;
using System.Text;


class FailureDescriptor
{
    public delegate void Failure();

    public Failure failure {get; set;}
    public float baseProbability { get; set; }
    public float probability { get; set; }

    public FailureDescriptor(Failure failure, float baseProbability)
    {
        this.failure = failure;
        this.baseProbability = baseProbability;
    }
}

