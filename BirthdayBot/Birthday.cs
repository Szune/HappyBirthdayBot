using System;

namespace BirthdayBot
{
    public class Birthday
    {
        public string Human { get; set; }
        public DateTime Date { get; set; }
        public bool Alerted { get; set; }

        public Birthday()
        {
            
        }
        public Birthday(string human, DateTime date, bool alerted = false)
        {
            Human = human;
            Date = date;
            Alerted = alerted;
        }

        public bool IsHappyBirthday(DateTime now)
        {
            if (Date.Day == now.Day && Date.Month == now.Month)
            {
                return true;
            }

            return false;
        }

        public void SetAlert()
        {
            Alerted = true;
        }

        public void ResetAlert()
        {
            Alerted = false;
        }
    }
}