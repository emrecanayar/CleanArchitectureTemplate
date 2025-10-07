using System.Security.Cryptography;

namespace Core.Helpers.Helpers
{
    public static class RandomPasswordHelper
    {
        private static int _dEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int _dEFAULT_MAX_PASSWORD_LENGTH = 10;
        private static string _pASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string _pASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string _pASSWORD_CHARS_NUMERIC = "0123456789";
        private static string _pASSWORD_CHARS_SPECIAL = "!@#$%^&*()_-=+{}:;\\<>?|,./`[]'";

        public static string? Generate()
        {
            return Generate(_dEFAULT_MIN_PASSWORD_LENGTH,
                            _dEFAULT_MAX_PASSWORD_LENGTH);
        }

        public static string? Generate(int length)
        {
            return Generate(length, length);
        }

        public static string? Generate(int minLength, int maxLength, bool special = true)
        {
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            {
                return null;
            }

            List<char[]> charGroups = new()
            {
            _pASSWORD_CHARS_LCASE.ToCharArray(),
            _pASSWORD_CHARS_UCASE.ToCharArray(),
            _pASSWORD_CHARS_NUMERIC.ToCharArray(),
            };

            if (special)
            {
                charGroups.Add(_pASSWORD_CHARS_SPECIAL.ToCharArray());
            }

            int[] charsLeftInGroup = new int[charGroups.Count];

            for (int i = 0; i < charsLeftInGroup.Length; i++)
            {
                charsLeftInGroup[i] = charGroups[i].Length;
            }

            int[] leftGroupsOrder = new int[charGroups.Count];

            for (int i = 0; i < leftGroupsOrder.Length; i++)
            {
                leftGroupsOrder[i] = i;
            }

            byte[] randomBytes = new byte[4];
            RandomNumberGenerator.Fill(randomBytes);
            int seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            Random random = new(seed);
            char[] password;
            if (minLength < maxLength)
            {
                password = new char[random.Next(minLength, maxLength + 1)];
            }
            else
            {
                password = new char[minLength];
            }

            int nextCharIdx;
            int nextGroupIdx;
            int nextLeftGroupsOrderIdx;
            int lastCharIdx;
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            for (int i = 0; i < password.Length; i++)
            {
                if (lastLeftGroupsOrderIdx == 0)
                {
                    nextLeftGroupsOrderIdx = 0;
                }
                else
                {
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);
                }

                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                if (lastCharIdx == 0)
                {
                    nextCharIdx = 0;
                }
                else
                {
                    nextCharIdx = random.Next(0, lastCharIdx + 1);
                }

                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                else
                {
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    charsLeftInGroup[nextGroupIdx]--;
                }

                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else
                {
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    lastLeftGroupsOrderIdx--;
                }
            }

            return new string(password);
        }
    }
}
