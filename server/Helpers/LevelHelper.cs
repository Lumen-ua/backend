namespace Server.Helpers
{
    public static class LevelHelper
    {
        public static string GetLevel(int exp)
        {
            if (exp >= 10) return "Легенда ЖКГ 🔥";
            if (exp >= 5) return "Комунальний майстер 💪";
            if (exp >= 1) return "Новачок 🟢";
            return "Без досвіду";
        }
    }
}
