using System;
using System.Collections.Generic;
using System.Text;


class FailureDescriptor
{
    public delegate void Failure();

    public Failure failure {get; set;}
    public int weight { get; set; }

    public FailureDescriptor(Failure failure, int weight)
    {
        this.failure = failure;
        this.weight = weight;
    }
}

