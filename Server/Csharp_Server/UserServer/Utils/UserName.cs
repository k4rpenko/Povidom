using PGAdminDAL;
using System;
using System.Text;

namespace UserServer.utils
{
    public class UserName
    {
        private static Random random = new Random();

        internal List<string> GenerateAdditionalNicknames(string nickname, AppDbContext context)
        {
            List<string> additionalNicknames = new List<string>();

            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));
            additionalNicknames.Add(GenerateRandomNickname(nickname, context));

            return additionalNicknames;
        }

        private string GenerateRandomNickname(string nick, AppDbContext context)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder nickname = new StringBuilder(nick);
            string newNickname;

            if (random.Next(0, 2) == 1)
            {
                nickname.Append('_');
            }

            do
            {
                nickname.Clear();
                nickname.Append(nick);
                int randomLength = random.Next(0, 6);
                for (int i = 0; i < randomLength; i++)
                {
                    nickname.Append(chars[random.Next(chars.Length)]);
                }
                newNickname = nickname.ToString();
            }
            while (context.Users.Any(u => u.UserName == newNickname));


            return nickname.ToString();
        }
    }
}
