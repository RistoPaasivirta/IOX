public class PID
{
    public float p, i, d;

    float integral;
    float lastError;

    public PID() { }

    public PID(float P, float I, float D)
    {
        p = P;
        i = I;
        d = D;
    }

    public float Update(float setpoint, float actual, float timeFrame)
    {
        float present = setpoint - actual;
        integral += present * timeFrame;
        float derivative = (present - lastError) / timeFrame;
        lastError = present;
        return present * p + integral * i + derivative * d;
    }
}