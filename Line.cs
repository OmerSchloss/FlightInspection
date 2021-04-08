namespace FlightInspection
{
    class Line
    {
        private float a;
        private float b;
        public Line(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        public float f(float x) { return a * x + b; }
    }
}
