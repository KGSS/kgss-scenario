//Author: Richard Bunt
using System;
using System.Collections.Generic;
using System.Text;

class FailureDescriptor
{
    public delegate void Failure(FailureDescriptor f);
    public delegate void FailureGarbage(FailureDescriptor f);


    private Failure itsFailure;
    public Failure failure 
    { get { KGSSLogger.Log("Failure Descriptor - Activating: " + name); return itsFailure; } 
        set { itsFailure = value; } }
    public FailureGarbage failureGarbage
    { get; set; }
    public int weight { get; set; }
    public string partInformation { get; set; }
    public string name { get; set; }

    public FailureDescriptor(Failure failure, FailureGarbage failureGarbage, int weight, string partInformation, string name)
    {
        this.failure = failure;
        this.failureGarbage = failureGarbage;
        this.weight = weight;
        this.partInformation = partInformation;
        this.name = name;
    }
}

