using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Infrastructure.Persistence.Helpers
{
    public static class Lexorank
    {
        private const string MIN_RANK = "00000000000000000000000000000000000000000000000000";
        private const string MAX_RANK = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz";
        private const string BASE_STR = "0123456789abcdefghijklmnopqrstuvwxyz";
        public static object FindRankBetween(string str1, string str2)
        {
            var a = str1.ToLower();
            var b = str2.ToLower();
            if (a.CompareTo(b) >= 0)
            {
                return 0;
            }
            var res = "";
            var index = 0;
            var isFound = false;
            while (true)
            {
                if (a.Length - 1 < index && b.Length - 1 < index)
                {
                    if (res == a)
                    {
                        res += FindLetterBetween('0', 'z');
                        isFound = true;
                    }
                    if (res == b)
                    {
                        //NOT FOUND nên không giải quyết nữa
                    }
                    break;
                }
                if (a.Length - 1 < index)
                {
                    //khi đã đi qua hết chuỗi a
                    if (b[index] == '0')
                    {
                        // ký tự tại index là 0 => không có ký tự nhỏ hơn
                        res += "0";
                        index++;
                        continue;
                    }
                    res += FindLetterBetween('0', b[index]); //tìm được ký tự nhỏ hơn

                    isFound = true;
                    break;
                }

                if (b.Length - 1 < index)
                {
                    //khi đã đi qua hết chuỗi b
                    if (a[index] == 'z')
                    {
                        // ký tự tại index là z => không có ký tự lớn hơn
                        res += "z";
                        index++;
                        continue;
                    }
                    if (a[index] == 'y')
                    {
                        res += "z";
                    }
                    else
                    {
                        res += FindLetterBetween(a[index], 'z'); //tìm được ký tự lớn hơn
                    }
                    isFound = true;
                    break;
                }

                if (a[index] == b[index])
                {
                    res += a[index];
                    index++;
                    continue;
                }
                if (a[index] < b[index])
                {
                    var betweenLetter = FindLetterBetween(a[index], b[index]);
                    if (betweenLetter != a[index] && betweenLetter != b[index])
                    {
                        res += betweenLetter;
                        isFound = true;
                        break;
                    }
                    res += betweenLetter;
                    index++;
                    continue;
                }
                //trường hợp a[index]>b[index]
                res += a[index];
                index++;
            }

            if (!isFound)
            {
                return false;
            }
            else
            {
                res = formatAgain(res);

                if (a.CompareTo(res) < 0 && res.CompareTo(b) < 0)
                {
                    return res;
                }
                else
                {
                    return false;
                }
            }
        }
        public static object FindNextRank(string str)
        {
            var nextRank = FindRankBetween(str, MAX_RANK);
            return nextRank;
        }
        public static object FindPreRank(string str)
        {
            var preRank = FindRankBetween(MIN_RANK, str);
            return preRank;
        }
        public static object genNewRank()
        {
            var newRank = FindRankBetween(MIN_RANK, MAX_RANK);
            return newRank;
        }

        private static char FindLetterBetween(char letterA, char letterB)
        {
            var index1 = BASE_STR.IndexOf(letterA);
            var index2 = BASE_STR.IndexOf(letterB);
            return BASE_STR[(index1 + index2) / 2];
        }

        private static string formatAgain(string res)
        {
            if (res.Length < 6)
            {
                var missingNumber = 6 - res.Length;
                for (int i = 0; i < missingNumber; i++)
                {
                    res += BASE_STR[getRandomBetween(10, 20)];
                }
            }

            //khi ký tự cuối bằng 0, thêm i để tránh trường hợp hết slot trong tương lai
            if (res[res.Length - 1] == '0')
            {
                res += "i";
            }
            return res;
        }
        private static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        private static int getRandomBetween(int min, int max)
        {
            return (int)Math.Round(GetRandomNumber(0, 1) * (max + 1 - min) + min);
        }

        public static List<object> CreateNewListRank(int n)
        {
            var newRanks = new List<object> { MIN_RANK, MAX_RANK };
            var createDone = false;
            while (!createDone)
            {
                //khi chưa tạo đủ số lượng n ranks thì vẫn tiếp tục lặp
                var saveNewRanks = new List<object>();
                for (int i = 0; i < newRanks.Count - 1; i++)
                {
                    saveNewRanks.Add(newRanks[i]);
                    if (!createDone)
                    {
                        var newRank = FindRankBetween((string)newRanks[i], (string)newRanks[i + 1]);
                        if ((string)newRank == "0" || (string)newRank == "False")
                        {

                        }
                        saveNewRanks.Add(newRank);

                        var newRankC = (string)newRank;
                        var newRanksI = (string)newRanks[i];
                        if (newRankC.CompareTo(newRanksI) <= 0 || newRankC.CompareTo(newRanksI) >= 0)
                        {

                        }
                    }
                    if (newRanks.Count + i + 1 - 2 == n)
                    {
                        //đã tạo đủ
                        createDone = true;
                    }
                }
                saveNewRanks.Add(MAX_RANK);
                newRanks = saveNewRanks;

            }

            return newRanks.GetRange(1, newRanks.Count - 2);
        }
    }
}
