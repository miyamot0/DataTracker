namespace DataTracker.Model
{
    class Therapist
    {
        public static Therapist CreateTherapist(string therapistName)
        {
            return new Therapist { TherapistsName = therapistName };
        }

        public string TherapistsName { get; set; }
    }
}
